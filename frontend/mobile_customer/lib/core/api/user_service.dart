import 'api_service.dart';

class UserService {
  final ApiService _api = ApiService();
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmNewPassword,
  }) async {
    final res = await _api.put(
      "Users/change-password",
      {
        "currentPassword": currentPassword,
        "newPassword": newPassword,
        "confirmNewPassword": confirmNewPassword,
      },
    );

    return;
  }
}
