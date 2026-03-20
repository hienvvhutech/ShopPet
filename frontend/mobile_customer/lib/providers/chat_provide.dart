import 'package:flutter/material.dart';
import 'package:mobile_customer/core/api/chat_service.dart';
import 'package:mobile_customer/data/models/chat_message.dart';
import 'package:mobile_customer/data/repositories/chat_repository.dart';

class ChatProvider extends ChangeNotifier {
  final ChatService _service = ChatService();
  final ChatRepository _repo = ChatRepository();

  List<ChatMessage> messages = [];

  String? _token;
  String? _adminId;

  Future<void> init({
    required String token,
    required String adminId,
  }) async {
    _token = token;
    _adminId = adminId;

    messages = await _repo.getChatHistory(
      token: token,
      adminId: adminId,
    );
    notifyListeners();

    await _service.connect(
      token: token,
      onMessage: (senderId, message) {
        messages.add(ChatMessage(
          senderId: senderId,
          receiverId: adminId,
          content: message,
          createdAt: DateTime.now(),
        ));
        notifyListeners();
      },
    );
  }

  Future<void> send(String text) async {
    if (text.trim().isEmpty || _adminId == null) return;

    await _service.sendMessage(_adminId!, text);

    messages.add(ChatMessage(
      senderId: "me",
      receiverId: _adminId!,
      content: text,
      createdAt: DateTime.now(),
    ));

    notifyListeners();
  }

  @override
  void dispose() {
    _service.disconnect();
    super.dispose();
  }
}
