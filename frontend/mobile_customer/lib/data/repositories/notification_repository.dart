import 'package:mobile_customer/core/api/api_service.dart';
import 'package:mobile_customer/data/models/notification_model.dart';

class NotificationRepository {
  final ApiService _api = ApiService();
  Future<List<NotificationModel>> getMyNotifications() async {
    final res = await _api.get("Notifications/my");
    if (res is List) {
      return res.map((e) => NotificationModel.fromJson(e)).toList();
    }
    throw Exception("Invalid notification response");
  }
}
