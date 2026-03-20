import 'package:flutter/material.dart';
import 'package:mobile_customer/data/models/notification_model.dart';
import 'package:mobile_customer/data/repositories/notification_repository.dart';

class NotificationProvider extends ChangeNotifier {
  final NotificationRepository _repo = NotificationRepository();
  List<NotificationModel> notifications = [];
  bool isLoading = false;
  String? error;
  int get unreadCount => notifications.where((n) => !n.isRead).length;
  Future<void> loadNotifications() async {
    isLoading = true;
    error = null;
    notifyListeners();
    try {
      notifications = await _repo.getMyNotifications();
    } catch (e) {
      error = e.toString();
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }
}
