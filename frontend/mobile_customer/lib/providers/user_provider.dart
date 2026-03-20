import 'package:flutter/material.dart';
import 'package:mobile_customer/core/api/user_service.dart';
import 'package:mobile_customer/data/repositories/user_repository.dart';

class UserProvider extends ChangeNotifier {
  final UserRepository _repo = UserRepository(UserService());
  bool isLoading = false;
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmNewPassword,
  }) async {
    isLoading = true;
    notifyListeners();
    try {
      await _repo.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
        confirmNewPassword: confirmNewPassword,
      );
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }
}
