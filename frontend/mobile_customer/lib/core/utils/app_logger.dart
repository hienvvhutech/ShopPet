import 'package:flutter/foundation.dart';

class AppLogger {
  static void d(String message, {String tag = "DEBUG"}) {
    if (kDebugMode) {
      debugPrint("🟢 [$tag] $message");
    }
  }

  static void i(String message, {String tag = "INFO"}) {
    if (kDebugMode) {
      debugPrint("🔵 [$tag] $message");
    }
  }

  static void w(String message, {String tag = "WARN"}) {
    if (kDebugMode) {
      debugPrint("🟠 [$tag] $message");
    }
  }

  static void e(
    String message, {
    String tag = "ERROR",
    Object? error,
    StackTrace? stackTrace,
  }) {
    if (kDebugMode) {
      debugPrint("🔴 [$tag] $message");
      if (error != null) {
        debugPrint("   ↳ error: $error");
      }
      if (stackTrace != null) {
        debugPrint("   ↳ stackTrace: $stackTrace");
      }
    }
  }
}
