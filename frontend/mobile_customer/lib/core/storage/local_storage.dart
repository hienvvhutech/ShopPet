import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class LocalStorage {
  static const _tokenKey = "auth_token";
  static const _addressListKey = "shipping_addresses";
  static const _defaultIndexKey = "default_address_index";
  static Future<void> saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, token);
  }

  static Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_tokenKey);
  }

  static Future<void> clearToken() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
  }

  static Future<void> cacheAddresses(List<String> addresses) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_addressListKey, jsonEncode(addresses));
  }

  static Future<List<String>> getCachedAddresses() async {
    final prefs = await SharedPreferences.getInstance();
    final jsonStr = prefs.getString(_addressListKey);
    if (jsonStr == null) return [];
    return List<String>.from(jsonDecode(jsonStr));
  }

  static Future<void> setDefaultIndex(int index) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setInt(_defaultIndexKey, index);
  }

  static Future<int?> getDefaultIndex() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_defaultIndexKey);
  }
}
