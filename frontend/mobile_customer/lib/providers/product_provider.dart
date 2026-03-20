import 'package:flutter/material.dart';
import 'package:mobile_customer/core/api/api_exception.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';
import 'package:mobile_customer/data/models/product.dart';
import 'package:mobile_customer/data/repositories/product_repository.dart';

class ProductProvider extends ChangeNotifier {
  final ProductRepository _repo = ProductRepository();
  List<Product> _allProducts = [];
  List<Product> products = [];
  bool isLoading = false;
  String? error;
  Future<void> loadProducts() async {
    AppLogger.i("Load products", tag: "PRODUCT_PROVIDER");
    isLoading = true;
    error = null;
    notifyListeners();
    try {
      _allProducts = await _repo.getProducts();
      products = List.from(_allProducts);
    } on ApiException catch (e) {
      error = e.message;
    } catch (e) {
      error = "Lỗi không xác định";
    }
    isLoading = false;
    notifyListeners();
  }

  void search(String keyword) {
    if (keyword.isEmpty) {
      products = List.from(_allProducts);
    } else {
      products = _allProducts
          .where(
            (p) => p.name.toLowerCase().contains(keyword.toLowerCase()),
          )
          .toList();
    }
    notifyListeners();
  }

  Product? getById(int id) {
    try {
      return _allProducts.firstWhere((p) => p.id == id);
    } catch (_) {
      return null;
    }
  }
}
