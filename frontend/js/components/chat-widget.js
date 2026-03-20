const userId = localStorage.getItem("userId");
const token = localStorage.getItem("token");
let adminId = null;

async function getAdminId() {
  try {
    const res = await fetch("http://localhost:5287/api/users/admin-id", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (res.ok) {
      const data = await res.json();
      adminId = data.id;
    } else {
      alert("Không lấy được adminId.");
    }
  } catch (err) {
    alert("Lỗi khi gọi API admin-id.");
  }
}

const chatButton = document.createElement("div");
chatButton.innerHTML = `💬`;
Object.assign(chatButton.style, {
  position: "fixed",
  bottom: "20px",
  right: "20px",
  backgroundColor: "#007bff",
  color: "#fff",
  borderRadius: "50%",
  width: "50px",
  height: "50px",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  fontSize: "24px",
  cursor: "pointer",
  boxShadow: "0 0 10px rgba(0,0,0,0.3)",
  zIndex: "999",
});

const chatBox = document.createElement("div");
Object.assign(chatBox.style, {
  position: "fixed",
  bottom: "80px",
  right: "20px",
  width: "300px",
  height: "400px",
  background: "#fff",
  border: "1px solid #ccc",
  borderRadius: "10px",
  boxShadow: "0 0 15px rgba(0,0,0,0.2)",
  display: "none",
  zIndex: "1000",
});
chatBox.innerHTML = `
  <div style="background:#007bff;color:white;padding:10px;font-weight:bold;border-radius:10px 10px 0 0;">
    💬 Hỗ trợ khách hàng
    <span style="float:right;cursor:pointer;" id="closeChat">✖</span>
  </div>
  <div style="height:300px; padding:10px; overflow-y:auto;" id="chatContent">
    <p><i>Đang tải tin nhắn...</i></p>
  </div>
  <div style="padding:10px;border-top:1px solid #ccc;">
    <input type="text" id="chatInput" placeholder="Nhập tin nhắn..." style="width:75%;padding:4px;" />
    <button id="sendBtn" class="btn btn-sm btn-primary">Gửi</button>
  </div>
`;

document.body.appendChild(chatButton);
document.body.appendChild(chatBox);

chatButton.onclick = async () => {
  await getAdminId();
  if (!adminId) return;
  chatBox.style.display = "block";
  await loadMessages();
};

document.getElementById("closeChat").onclick = () =>
  (chatBox.style.display = "none");

document.getElementById("sendBtn").onclick = async () => {
  const input = document.getElementById("chatInput");
  const text = input.value.trim();
  if (!text || !adminId) return;

  try {
    const res = await fetch("http://localhost:5287/api/chat/send", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        receiverId: adminId,
        message: text,
      }),
    });

    if (res.ok) {
      input.value = "";
      await loadMessages();
    } else {
      alert("Gửi thất bại.");
    }
  } catch (e) {
    alert("Lỗi kết nối.");
  }
};

async function loadMessages() {
  const content = document.getElementById("chatContent");
  content.innerHTML = "<p><i>Đang tải tin nhắn...</i></p>";
  try {
    const res = await fetch(`http://localhost:5287/api/chat/${adminId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    const messages = await res.json();
    content.innerHTML = "";
    messages.forEach((m) => {
      const isSender = m.senderId === userId;
      const align = isSender ? "right" : "left";
      const name = isSender ? "Bạn" : "Admin";
      const time = new Date(m.timestamp).toLocaleString("vi-VN", {
        hour: "2-digit",
        minute: "2-digit",
        day: "2-digit",
        month: "2-digit",
      });

      content.innerHTML += `
    <div style="text-align:${align};margin:5px 0;">
      <b>${name}</b> <span style="font-size:0.75em;color:gray;">(${time})</span>:<br/>
      ${m.message}
    </div>`;
    });

    content.scrollTop = content.scrollHeight;
  } catch (err) {
    content.innerHTML = "<p><i>Lỗi khi tải tin nhắn</i></p>";
  }
}
