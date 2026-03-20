import 'package:flutter/material.dart';
import 'package:mobile_customer/core/utils/app_logger.dart';
import 'package:mobile_customer/data/models/cart_item.dart';
import 'package:mobile_customer/data/models/product.dart';

class CartProvider extends ChangeNotifier {
  final List<CartItem> _items = [];
  List<CartItem> get items => _items;
  int get totalPrice => _items.fold(0, (sum, item) => sum + item.totalPrice);
  void addToCart(Product product) {
    AppLogger.i("Add to cart: ${product.name}", tag: "CART_PROVIDER");
    final index = _items.indexWhere((e) => e.productId == product.id);
    if (index >= 0) {
      _items[index].quantity++;
    } else {
      _items.add(
        CartItem(
          productId: product.id,
          name: product.name,
          price: product.price,
          imageUrl: product.imageUrl,
        ),
      );
    }
    notifyListeners();
  }

  void updateQty(int productId, int qty) {
    final index = _items.indexWhere((e) => e.productId == productId);
    if (index >= 0) {
      _items[index].quantity = qty;
      if (qty <= 0) {
        _items.removeAt(index);
      }
      notifyListeners();
    }
  }

  void removeItem(int productId) {
    _items.removeWhere((e) => e.productId == productId);
    notifyListeners();
  }

  void clear() {
    AppLogger.i("Clear cart", tag: "CART_PROVIDER");
    _items.clear();
    notifyListeners();
  }
}
