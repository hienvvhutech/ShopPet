import 'package:mobile_customer/core/storage/local_storage.dart';
import 'package:mobile_customer/core/api/api_service.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';

class AuthRepository {
  final ApiService _api = ApiService();
  Future<void> login(String email, String password) async {
    AppLogger.i("Login: $email", tag: "AUTH_REPO");
    final res = await _api.post(
      "Auth/login",
      {
        "email": email,
        "password": password,
      },
    );
    final token = res["token"];
    if (token == null) {
      throw Exception("Không có token");
    }
    await LocalStorage.saveToken(token);
  }

  Future<void> register({
    required String email,
    required String password,
    String? fullName,
    String? phone,
  }) async {
    AppLogger.i("Register: $email", tag: "AUTH_REPO");
    final res = await _api.post(
      "Auth/register",
      {
        "email": email,
        "password": password,
        "fullName": fullName,
        "phone": phone,
      },
    );
    final token = res["token"];
    if (token == null) {
      throw Exception("Không có token");
    }
    await LocalStorage.saveToken(token);
  }

  Future<void> logout() async {
    AppLogger.i("Logout", tag: "AUTH_REPO");
    await LocalStorage.clearToken();
  }
}
