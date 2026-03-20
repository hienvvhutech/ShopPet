import 'package:mobile_customer/core/api/api_service.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';
import 'package:mobile_customer/data/models/product.dart';

class ProductRepository {
  final ApiService _api = ApiService();
  Future<List<Product>> getProducts() async {
    AppLogger.i("Fetch products", tag: "PRODUCT_REPO");
    final res = await _api.get("Products");
    if (res is List) {
      return res
          .map((e) => Product.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    if (res is Map && res.containsKey("data")) {
      final List list = res["data"];
      return list
          .map((e) => Product.fromJson(e as Map<String, dynamic>))
          .toList();
    }

    AppLogger.w("Products API response invalid", tag: "PRODUCT_REPO");
    return [];
  }

  Future<Product?> getProductById(int id) async {
    AppLogger.i("Fetch product $id", tag: "PRODUCT_REPO");
    final res = await _api.get("Products/$id");
    if (res is Map<String, dynamic>) {
      return Product.fromJson(res);
    }
    return null;
  }
}
