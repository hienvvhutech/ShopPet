import 'package:flutter/material.dart';
import 'package:mobile_customer/core/api/api_exception.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';
import 'package:mobile_customer/data/models/user.dart';
import 'package:mobile_customer/data/repositories/auth_repository.dart';

class AuthProvider extends ChangeNotifier {
  final AuthRepository _repo = AuthRepository();
  User? user;
  bool isLoading = false;
  String? error;
  bool get isLoggedIn => user != null;
  Future<bool> login(String email, String password) async {
    AppLogger.i("Login start", tag: "AUTH_PROVIDER");
    isLoading = true;
    error = null;
    notifyListeners();
    try {
      await _repo.login(email, password);
      user = User(
        fullName: "Khách hàng",
        email: email,
        phone: "0123456789",
      );
      AppLogger.i("Login success", tag: "AUTH_PROVIDER");
      return true;
    } on ApiException catch (e) {
      error = e.message;
      AppLogger.e("Login failed", tag: "AUTH_PROVIDER", error: e);
      return false;
    } catch (e) {
      error = "Lỗi không xác định";
      AppLogger.e("Unknown error", tag: "AUTH_PROVIDER", error: e);
      return false;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<bool> register({
    required String email,
    required String password,
    String? fullName,
    String? phone,
  }) async {
    AppLogger.i("Register start", tag: "AUTH_PROVIDER");
    isLoading = true;
    error = null;
    notifyListeners();
    try {
      await _repo.register(
        email: email,
        password: password,
        fullName: fullName,
        phone: phone,
      );
      user = User.guest();
      return true;
    } on ApiException catch (e) {
      error = e.message;
      return false;
    } catch (e) {
      error = "Lỗi không xác định";
      return false;
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<void> logout() async {
    AppLogger.i("Logout", tag: "AUTH_PROVIDER");
    await _repo.logout();
    user = null;
    notifyListeners();
  }
}
