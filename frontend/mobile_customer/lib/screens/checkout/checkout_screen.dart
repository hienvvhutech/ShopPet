import 'package:flutter/material.dart';
import 'package:mobile_customer/data/repositories/order_repository.dart';
import 'package:mobile_customer/providers/auth_provider.dart';
import 'package:mobile_customer/providers/cart_provider.dart';
import 'package:mobile_customer/routes/app_router.dart';
import 'package:provider/provider.dart';

class CheckoutScreen extends StatefulWidget {
  const CheckoutScreen();
  @override
  State<CheckoutScreen> createState() => _CheckoutScreenState();
}

class _CheckoutScreenState extends State<CheckoutScreen> {
  final TextEditingController _addressCtrl = TextEditingController();
  final OrderRepository _orderRepo = OrderRepository();
  String paymentMethod = "COD";
  bool isLoading = false;
  List<Map<String, dynamic>> addresses = [];
  int? selectedAddressId;
  @override
  void initState() {
    super.initState();
    _loadAddresses();
  }

  Future<void> _loadAddresses() async {
    try {
      final res = await _orderRepo.getAddresses();
      final list = List<Map<String, dynamic>>.from(res);
      if (list.isEmpty) return;
      final defaultAddress = list.firstWhere(
        (e) => e["isDefault"] == true,
        orElse: () => list.first,
      );
      setState(() {
        addresses = list;
        selectedAddressId = defaultAddress["id"];
        _addressCtrl.text = defaultAddress["addressLine"];
      });
    } catch (e) {}
  }

  @override
  void dispose() {
    _addressCtrl.dispose();
    super.dispose();
  }

  Future<void> _submitOrder(BuildContext context) async {
    final cartProvider = context.read<CartProvider>();
    final auth = context.read<AuthProvider>();
    final user = auth.user;
    if (selectedAddressId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Vui lòng chọn địa chỉ giao hàng")),
      );
      return;
    }
    setState(() => isLoading = true);
    try {
      await _orderRepo.createOrder(
        customerName: user?.fullName ?? "Khách hàng",
        phone: user?.phone ?? "0123456789",
        email: user?.email ?? "guest@shoppet.com",
        shippingAddress: _addressCtrl.text,
        items: cartProvider.items,
      );
      cartProvider.clear();
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Đặt hàng thành công")),
      );
      Navigator.pushNamedAndRemoveUntil(
        context,
        AppRouter.home,
        (route) => false,
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Lỗi đặt hàng: $e")),
      );
    } finally {
      if (mounted) setState(() => isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final cartProvider = context.watch<CartProvider>();

    return Scaffold(
      appBar: AppBar(
        title: const Text("Thanh toán"),
      ),
      body: cartProvider.items.isEmpty
          ? const Center(child: Text("Giỏ hàng trống"))
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    "Địa chỉ giao hàng",
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  TextField(
                    controller: _addressCtrl,
                    enabled: false,
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                    ),
                  ),
                  if (addresses.isNotEmpty) ...[
                    const SizedBox(height: 12),
                    const Text(
                      "Chọn địa chỉ khác",
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(height: 8),
                    ...addresses.map(
                      (addr) => RadioListTile<int>(
                        value: addr["id"],
                        groupValue: selectedAddressId,
                        title: Text(addr["addressLine"]),
                        subtitle: addr["isDefault"] == true
                            ? const Text(
                                "Mặc định",
                                style: TextStyle(color: Colors.green),
                              )
                            : null,
                        onChanged: (value) {
                          setState(() {
                            selectedAddressId = value;
                            _addressCtrl.text = addr["addressLine"];
                          });
                        },
                      ),
                    ),
                  ],
                  const SizedBox(height: 16),
                  const Text(
                    "Phương thức thanh toán",
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  RadioListTile(
                    value: "COD",
                    groupValue: paymentMethod,
                    onChanged: (value) {
                      setState(() => paymentMethod = value!);
                    },
                    title: const Text("Thanh toán khi nhận hàng (COD)"),
                  ),
                  RadioListTile(
                    value: "BANK",
                    groupValue: paymentMethod,
                    onChanged: (value) {
                      setState(() => paymentMethod = value!);
                    },
                    title: const Text("Chuyển khoản ngân hàng"),
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    "Đơn hàng",
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  ...cartProvider.items.map(
                    (item) => ListTile(
                      title: Text(item.name),
                      trailing: Text(
                        "x${item.quantity}  ${item.totalPrice} đ",
                      ),
                    ),
                  ),
                  const Divider(),
                  Align(
                    alignment: Alignment.centerRight,
                    child: Text(
                      "Tổng tiền: ${cartProvider.totalPrice} đ",
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: Colors.red,
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),
                  SizedBox(
                    width: double.infinity,
                    height: 48,
                    child: ElevatedButton(
                      onPressed: isLoading ? null : () => _submitOrder(context),
                      child: isLoading
                          ? const CircularProgressIndicator(
                              color: Colors.white,
                            )
                          : const Text("Đặt hàng"),
                    ),
                  ),
                ],
              ),
            ),
    );
  }
}
