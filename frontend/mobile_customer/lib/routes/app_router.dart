import 'package:flutter/material.dart';
import 'package:mobile_customer/screens/account/change_password_screen.dart';
import 'package:mobile_customer/screens/account/support_screen.dart';
import 'package:mobile_customer/screens/auth/login_screen.dart';
import 'package:mobile_customer/screens/auth/register_screen.dart';
import 'package:mobile_customer/screens/cart/cart_screen.dart';
import 'package:mobile_customer/screens/checkout/checkout_screen.dart';
import 'package:mobile_customer/screens/orders/orders_screen.dart';
import 'package:mobile_customer/screens/product/product_detail_screen.dart';
import 'package:mobile_customer/screens/root/root_screen.dart';
import 'package:mobile_customer/screens/splash/splash_screen.dart';

class AppRouter {
  static const String splash = '/splash';
  static const String root = '/';
  static const String login = '/login';
  static const String register = '/register';
  static const String home = '/home';
  static const String cart = '/cart';
  static const String checkout = '/checkout';
  static const String orders = '/orders';
  static const String productDetail = '/product-detail';
  static const String updateProfile = '/update-profile';
  static const String supportChat = '/support-chat';
  static Route<dynamic> generateRoute(RouteSettings settings) {
    switch (settings.name) {
      case splash:
        return MaterialPageRoute(
          builder: (_) => const SplashScreen(),
        );
      case root:
        return MaterialPageRoute(
          builder: (_) => const RootScreen(),
        );
      case login:
        return MaterialPageRoute(
          builder: (_) => const LoginScreen(),
        );
      case register:
        return MaterialPageRoute(
          builder: (_) => const RegisterScreen(),
        );
      case cart:
        return MaterialPageRoute(
          builder: (_) => const CartScreen(),
        );
      case orders:
        return MaterialPageRoute(
          builder: (_) => const OrdersScreen(),
        );
      case checkout:
        return MaterialPageRoute(
          builder: (_) => const CheckoutScreen(),
        );
      case productDetail:
        final productId = settings.arguments as int;
        return MaterialPageRoute(
          builder: (_) => ProductDetailScreen(productId: productId),
        );
      case updateProfile:
        return MaterialPageRoute(
          builder: (_) => const ChangePasswordScreen(),
        );
      case supportChat:
        return MaterialPageRoute(
          builder: (_) => const SupportChatScreen(),
        );
      default:
        return MaterialPageRoute(
          builder: (_) => const RootScreen(),
        );
    }
  }
}
