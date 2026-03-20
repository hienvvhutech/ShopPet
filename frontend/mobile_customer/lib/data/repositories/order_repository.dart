import 'package:mobile_customer/core/api/api_service.dart';
import 'package:mobile_customer/data/models/cart_item.dart';

class OrderRepository {
  final ApiService _api = ApiService();
  Future<void> createOrder({
    required String customerName,
    required String phone,
    required String email,
    required String shippingAddress,
    String? note,
    required List<CartItem> items,
  }) async {
    await _api.post(
      "Orders",
      {
        "CustomerName": customerName,
        "Phone": phone,
        "Email": email,
        "ShippingAddress": shippingAddress,
        "Note": note ?? "",
        "OrderItems": items
            .map(
              (e) => {
                "ProductId": e.productId,
                "Quantity": e.quantity,
                "UnitPrice": e.price,
              },
            )
            .toList(),
      },
    );
  }

  Future<List<dynamic>> getOrders() async {
    return await _api.get("Orders");
  }

  Future<List<dynamic>> getAddresses() async {
    return await _api.get("Addresses");
  }

  Future<Map<String, dynamic>> getOrderDetail(int id) async {
    final res = await _api.get("Orders/$id");
    return Map<String, dynamic>.from(res);
  }

  Future<void> cancelOrder(int orderId) async {
    await _api.delete("Orders/$orderId");
  }
}
