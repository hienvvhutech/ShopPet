import 'package:mobile_customer/core/api/user_service.dart';

class UserRepository {
  final UserService _service;

  UserRepository(this._service);

  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmNewPassword,
  }) {
    return _service.changePassword(
      currentPassword: currentPassword,
      newPassword: newPassword,
      confirmNewPassword: confirmNewPassword,
    );
  }
}
