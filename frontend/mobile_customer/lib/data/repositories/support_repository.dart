import 'package:mobile_customer/core/api/support_service.dart';

class SupportRepository {
  final SupportService _service = SupportService();
  Future<void> sendFeedback(String title, String content) {
    return _service.sendFeedback(
      title: title,
      content: content,
    );
  }
}
