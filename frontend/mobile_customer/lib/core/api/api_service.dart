import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:mobile_customer/core/storage/local_storage.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';
import 'api_config.dart';
import 'api_exception.dart';

class ApiService {
  Future<dynamic> get(String endpoint) async {
    AppLogger.i("GET /$endpoint", tag: "API");
    try {
      final token = await LocalStorage.getToken();
      final uri = Uri.parse("${ApiConfig.baseUrl}/$endpoint");
      final response = await http.get(
        uri,
        headers: {
          "Content-Type": "application/json",
          "Accept": "application/json",
          if (token != null) "Authorization": "Bearer $token",
        },
      ).timeout(const Duration(milliseconds: ApiConfig.timeout));
      return _handleResponse(response, endpoint);
    } on SocketException {
      throw ApiException(
        "Không có kết nối mạng",
      );
    } on HttpException {
      throw ApiException(
        "Lỗi giao thức HTTP",
      );
    } on FormatException {
      throw ApiException(
        "Dữ liệu trả về không hợp lệ",
      );
    } catch (e, stack) {
      AppLogger.e(
        "GET /$endpoint failed",
        tag: "API",
        error: e,
        stackTrace: stack,
      );
      throw ApiException("Lỗi không xác định: $e");
    }
  }

  Future<dynamic> delete(String endpoint) async {
    final token = await LocalStorage.getToken();
    final uri = Uri.parse("${ApiConfig.baseUrl}/$endpoint");
    final response = await http.delete(
      uri,
      headers: {
        "Content-Type": "application/json",
        if (token != null) "Authorization": "Bearer $token",
      },
    );
    return _handleResponse(response, endpoint);
  }

  Future<dynamic> post(String endpoint, dynamic body) async {
    AppLogger.i("POST /$endpoint", tag: "API");
    try {
      final token = await LocalStorage.getToken();
      final uri = Uri.parse("${ApiConfig.baseUrl}/$endpoint");

      final response = await http
          .post(
            uri,
            headers: {
              "Content-Type": "application/json",
              "Accept": "application/json",
              if (token != null) "Authorization": "Bearer $token",
            },
            body: jsonEncode(body),
          )
          .timeout(const Duration(milliseconds: ApiConfig.timeout));

      return _handleResponse(response, endpoint);
    } on SocketException {
      throw ApiException("Không có kết nối mạng");
    } on FormatException {
      throw ApiException("Dữ liệu gửi lên không hợp lệ");
    } catch (e, stack) {
      AppLogger.e(
        "POST /$endpoint failed",
        tag: "API",
        error: e,
        stackTrace: stack,
      );
      throw ApiException("Lỗi không xác định: $e");
    }
  }

  dynamic _handleResponse(http.Response response, String endpoint) {
    final status = response.statusCode;
    AppLogger.d(
      "Response [$status] /$endpoint",
      tag: "API",
    );
    if (status >= 200 && status < 300) {
      if (response.body.isEmpty) return null;
      return jsonDecode(response.body);
    }
    String message = "Lỗi server";
    try {
      final body = jsonDecode(response.body);
      message =
          body["message"] ?? body["error"] ?? body["title"] ?? response.body;
    } catch (_) {
      message = response.body;
    }
    AppLogger.w(
      "API error [$status] /$endpoint → $message",
      tag: "API",
    );
    throw ApiException(
      message,
      statusCode: status,
    );
  }

  Future<dynamic> put(String endpoint, Map<String, dynamic> body) async {
    AppLogger.i("PUT /$endpoint", tag: "API");
    try {
      final token = await LocalStorage.getToken();
      final uri = Uri.parse("${ApiConfig.baseUrl}/$endpoint");
      final response = await http
          .put(
            uri,
            headers: {
              "Content-Type": "application/json",
              "Accept": "application/json",
              if (token != null) "Authorization": "Bearer $token",
            },
            body: jsonEncode(body),
          )
          .timeout(const Duration(milliseconds: ApiConfig.timeout));
      return _handleResponse(response, endpoint);
    } on SocketException {
      throw ApiException("Không có kết nối mạng");
    } catch (e) {
      throw ApiException(e.toString());
    }
  }
}
