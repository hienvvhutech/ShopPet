import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:mobile_customer/core/api/api_config.dart';
import 'package:mobile_customer/data/models/chat_message.dart';

class ChatRepository {
  Future<List<ChatMessage>> getChatHistory({
    required String token,
    required String adminId,
  }) async {
    final res = await http.get(
      Uri.parse("${ApiConfig.baseUrl}/api/chat/$adminId"),
      headers: {
        "Authorization": "Bearer $token",
      },
    );
    if (res.statusCode != 200) {
      throw Exception("Không load được chat");
    }
    final List data = json.decode(res.body);
    return data.map((e) => ChatMessage.fromJson(e)).toList();
  }
}
