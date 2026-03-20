const token = localStorage.getItem("token");
const API_GET_USERS = "http://localhost:5287/api/users/customers";
const API_CHAT = "http://localhost:5287/api/chat/";

let connection = null;

function initSignalR() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5287/chatHub", {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMessage", (senderId, role, message, time) => {
    console.log("📩", senderId, message);

    if (senderId === selectedUserId) {
      document.getElementById("chatMessages").innerHTML += `
        <div style="text-align:left;margin:5px 0">
          <b>${role}</b> (${time}): ${message}
        </div>`;
    }
  });

  connection.start().then(() => {
    console.log("✅ Admin SignalR connected");
  });
}

let selectedUserId = null;
let unreadCounts = {};

async function loadUsers() {
  const list = document.getElementById("userList");
  list.innerHTML = "<p>Đang tải...</p>";

  try {
    const res = await fetch(API_GET_USERS, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const users = await res.json();

    await fetchUnreadCounts();

    list.innerHTML = users
      .map((u) => {
        const name = u.fullName || u.email;
        const unread = unreadCounts[u.id] || 0;
        const badge =
          unread > 0
            ? `<span class="badge bg-danger ms-2">${unread}</span>`
            : "";
        return `
        <div class="user-item d-flex justify-content-between align-items-center" onclick="selectUser('${u.id}', '${name}')">
          <span>👤 ${name}</span>
          ${badge}
        </div>`;
      })
      .join("");
  } catch (err) {
    list.innerHTML = "<p>Lỗi tải danh sách</p>";
  }
}

async function fetchUnreadCounts() {
  try {
    const res = await fetch(API_CHAT + "unread-counts", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (res.ok) {
      const data = await res.json();
      unreadCounts = {};

      for (const item of data) {
        unreadCounts[item.userId] = item.unreadCount;
      }
    }
  } catch (e) {
    console.warn("Không thể lấy số tin chưa đọc");
  }
}

async function selectUser(userId, name) {
  selectedUserId = userId;
  document.getElementById("chatTitle").innerText = `💬 Đang chat với: ${name}`;

  unreadCounts[userId] = 0;
  await loadMessages(userId);
  await loadUsers();
}

async function loadMessages(userId) {
  const content = document.getElementById("chatMessages");
  content.innerHTML = "<p><i>Đang tải...</i></p>";

  try {
    const res = await fetch(API_CHAT + userId, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const messages = await res.json();
    content.innerHTML = messages
      .map((m) => {
        const isSender = m.senderId === userId;
        const align = isSender ? "left" : "right";
        const name = isSender ? "Khách" : "Admin";
        const time = new Date(m.timestamp).toLocaleString("vi-VN", {
          hour: "2-digit",
          minute: "2-digit",
          day: "2-digit",
          month: "2-digit",
        });

        return `
      <div style="text-align:${align}; margin:5px 0;">
        <b>${name}</b> <span style="font-size: 0.75em; color: gray;">(${time})</span>:
        ${m.content || m.message || "(Không có nội dung)"}
      </div>`;
      })
      .join("");

    content.scrollTop = content.scrollHeight;
  } catch (err) {
    content.innerHTML = "<p><i>Lỗi khi tải tin nhắn</i></p>";
  }
}

async function sendMessage() {
  const input = document.getElementById("chatInput");
  const text = input.value.trim();
  if (!text || !selectedUserId) return;

  try {
    // 🔥 GỬI QUA SIGNALR
    await connection.invoke("SendMessageToUser", selectedUserId, text);

    input.value = "";
    input.focus();

    // append UI phía admin luôn
    const content = document.getElementById("chatMessages");
    content.innerHTML += `
      <div style="text-align:right; margin:5px 0;">
        <b>Admin</b>
        <span style="font-size: 0.75em; color: gray;">(${new Date().toLocaleTimeString()})</span>:
        ${text}
      </div>
    `;
    content.scrollTop = content.scrollHeight;
  } catch (e) {
    console.error("❌ Send message error", e);
    alert("Gửi tin nhắn thất bại");
  }
}

document.addEventListener("DOMContentLoaded", () => {
  loadUsers();
  initSignalR();
  setInterval(loadUsers, 15000);
});
