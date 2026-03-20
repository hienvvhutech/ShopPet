import 'package:flutter/material.dart';
import 'package:mobile_customer/data/repositories/support_repository.dart';

class SupportProvider extends ChangeNotifier {
  final SupportRepository _repo = SupportRepository();
  bool isLoading = false;
  Future<void> sendFeedback({
    required String title,
    required String content,
  }) async {
    isLoading = true;
    notifyListeners();
    await _repo.sendFeedback(title, content);
    isLoading = false;
    notifyListeners();
  }
}
