import 'package:flutter/material.dart';
import 'package:mobile_customer/providers/auth_provider.dart';
import 'package:mobile_customer/screens/account/account_screen.dart';
import 'package:mobile_customer/screens/auth/guestAuthScreen.dart';
import 'package:mobile_customer/screens/cart/cart_screen.dart';
import 'package:mobile_customer/screens/home/home_screen.dart';
import 'package:mobile_customer/screens/orders/orders_screen.dart';
import 'package:provider/provider.dart';

class RootScreen extends StatefulWidget {
  const RootScreen();
  @override
  State<RootScreen> createState() => _RootScreenState();
}

class _RootScreenState extends State<RootScreen> {
  int _currentIndex = 0;
  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final isLoggedIn = auth.isLoggedIn;
    final List<Widget> screens = isLoggedIn
        ? [
            const HomeScreen(),
            const CartScreen(),
            const OrdersScreen(),
            const AccountScreen(),
          ]
        : [
            const HomeScreen(),
            const GuestAuthScreen(),
          ];
    final items = isLoggedIn
        ? const [
            BottomNavigationBarItem(
              icon: Icon(Icons.home_outlined),
              label: "Trang chủ",
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.shopping_cart_outlined),
              label: "Giỏ hàng",
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.receipt_long),
              label: "Đơn hàng",
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.person_outline),
              label: "Tôi",
            ),
          ]
        : const [
            BottomNavigationBarItem(
              icon: Icon(Icons.home_outlined),
              label: "Trang chủ",
            ),
            BottomNavigationBarItem(
              icon: Icon(Icons.login),
              label: "Đăng nhập",
            ),
          ];

    return Scaffold(
      body: screens[_currentIndex],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        type: BottomNavigationBarType.fixed,
        selectedItemColor: Colors.orange,
        unselectedItemColor: Colors.grey,
        items: items,
        onTap: (index) {
          setState(() => _currentIndex = index);
        },
      ),
    );
  }
}
