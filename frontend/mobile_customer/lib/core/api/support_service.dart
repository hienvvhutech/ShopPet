import 'package:http/http.dart' as http;
import 'api_config.dart';
import 'api_service.dart';

class SupportService {
  final ApiService _api = ApiService();
  Future<void> sendFeedback({
    required String title,
    required String content,
  }) async {
    final res = await _api.post(
      "Support/feedback",
      {
        "title": title,
        "content": content,
      },
    );
    if (res is Map && res.containsKey("error")) {
      throw Exception(res["error"]);
    }
  }
}
