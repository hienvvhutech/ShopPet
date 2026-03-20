import 'package:signalr_core/signalr_core.dart';
import 'api_config.dart';

class ChatService {
  HubConnection? _connection;
  Future<void> connect({
    required String token,
    required Function(String senderId, String message) onMessage,
  }) async {
    _connection = HubConnectionBuilder()
        .withUrl(
          "${ApiConfig.baseUrl}/chatHub",
          HttpConnectionOptions(
            accessTokenFactory: () async => token,
          ),
        )
        .withAutomaticReconnect()
        .build();
    _connection!.on("ReceiveMessage", (args) {
      final senderId = args![0] as String;
      final message = args[1] as String;
      onMessage(senderId, message);
    });
    await _connection!.start();
  }

  Future<void> sendMessage(String receiverId, String message) async {
    if (_connection == null) return;
    await _connection!.invoke(
      "SendMessage",
      args: [receiverId, message],
    );
  }

  Future<void> disconnect() async {
    await _connection?.stop();
  }
}
