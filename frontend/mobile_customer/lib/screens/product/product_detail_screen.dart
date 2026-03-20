import 'package:flutter/material.dart';
import 'package:mobile_customer/data/models/product.dart';
import 'package:mobile_customer/providers/auth_provider.dart';
import 'package:mobile_customer/providers/cart_provider.dart';
import 'package:mobile_customer/providers/product_provider.dart';
import 'package:mobile_customer/routes/app_router.dart';
import 'package:provider/provider.dart';

class ProductDetailScreen extends StatelessWidget {
  final int productId;
  const ProductDetailScreen({
    super.key,
    required this.productId,
  });
  @override
  Widget build(BuildContext context) {
    final productProvider = context.watch<ProductProvider>();
    final cartProvider = context.read<CartProvider>();
    final authProvider = context.watch<AuthProvider>();
    final Product? product = productProvider.getById(productId);
    if (product == null) {
      return const Scaffold(
        body: Center(child: Text("Không tìm thấy sản phẩm")),
      );
    }
    return Scaffold(
      appBar: AppBar(
        title: Text(product.name),
      ),
      bottomNavigationBar: _buildBottomBar(
        context,
        product,
        cartProvider,
        authProvider,
      ),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            AspectRatio(
              aspectRatio: 1,
              child: product.imageUrl.isNotEmpty
                  ? Image.network(
                      product.imageUrl,
                      fit: BoxFit.cover,
                      errorBuilder: (_, __, ___) =>
                          const Icon(Icons.image_not_supported, size: 80),
                    )
                  : const Icon(Icons.image, size: 80),
            ),
            Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    product.name,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    "${product.price} đ",
                    style: const TextStyle(
                      fontSize: 20,
                      color: Colors.red,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),
                  Text(
                    product.description ?? "Chưa có mô tả",
                    style: const TextStyle(fontSize: 14),
                  ),
                  const SizedBox(height: 12),
                  Text(
                    "Kho: ${product.stock}",
                    style: const TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBottomBar(
    BuildContext context,
    Product product,
    CartProvider cartProvider,
    AuthProvider authProvider,
  ) {
    return Container(
      padding: const EdgeInsets.all(8),
      height: 64,
      child: Row(
        children: [
          Expanded(
            child: OutlinedButton(
              onPressed: () {
                cartProvider.addToCart(product);
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text("Đã thêm vào giỏ"),
                    duration: Duration(milliseconds: 500),
                  ),
                );
              },
              child: const Text("Thêm giỏ"),
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: ElevatedButton(
              onPressed: () {
                cartProvider.addToCart(product);

                if (!authProvider.isLoggedIn) {
                  Navigator.pushNamed(
                    context,
                    AppRouter.login,
                    arguments: AppRouter.checkout,
                  );
                } else {
                  Navigator.pushNamed(context, AppRouter.checkout);
                }
              },
              child: const Text("Mua ngay"),
            ),
          ),
        ],
      ),
    );
  }
}
