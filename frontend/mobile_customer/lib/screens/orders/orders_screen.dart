import 'package:flutter/material.dart';
import 'package:mobile_customer/data/repositories/order_repository.dart';
import 'package:mobile_customer/screens/orders/order_detail_screen.dart';

class OrdersScreen extends StatefulWidget {
  const OrdersScreen({super.key});
  @override
  State<OrdersScreen> createState() => _OrdersScreenState();
}

class _OrdersScreenState extends State<OrdersScreen> {
  final OrderRepository _repo = OrderRepository();
  bool isLoading = true;
  String? error;
  List orders = [];
  @override
  void initState() {
    super.initState();
    _loadOrders();
  }

  Future<void> _loadOrders() async {
    try {
      final res = await _repo.getOrders();
      setState(() {
        orders = res;
      });
    } catch (e) {
      setState(() {
        error = e.toString();
      });
    } finally {
      setState(() => isLoading = false);
    }
  }

  Future<void> _confirmCancel(int orderId) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Huỷ đơn hàng"),
        content: const Text("Bạn có chắc muốn huỷ đơn này không?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text("Không"),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text(
              "Huỷ đơn",
              style: TextStyle(color: Colors.red),
            ),
          ),
        ],
      ),
    );
    if (confirm == true) {
      await _repo.cancelOrder(orderId);
      _loadOrders();
    }
  }

  Color _statusColor(String status) {
    switch (status) {
      case "Pending":
        return Colors.orange;
      case "Confirmed":
        return Colors.blue;
      case "Shipping":
        return Colors.purple;
      case "Done":
        return Colors.green;
      case "Cancelled":
        return Colors.red;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Đơn hàng của tôi"),
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : error != null
              ? Center(child: Text(error!))
              : orders.isEmpty
                  ? const Center(child: Text("Chưa có đơn hàng"))
                  : ListView.builder(
                      itemCount: orders.length,
                      itemBuilder: (_, index) {
                        final order = orders[index];
                        final status = order["status"] ?? "Pending";
                        final canCancel = status == "Pending";

                        return InkWell(
                          onTap: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (_) => OrderDetailScreen(
                                  orderId: order["id"],
                                ),
                              ),
                            );
                          },
                          child: Card(
                            margin: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 6,
                            ),
                            child: Padding(
                              padding: const EdgeInsets.all(12),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Text(
                                        "Đơn Hàng: #${order["id"]}",
                                        style: const TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontSize: 16,
                                        ),
                                      ),
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                          horizontal: 10,
                                          vertical: 4,
                                        ),
                                        decoration: BoxDecoration(
                                          color: _statusColor(status),
                                          borderRadius:
                                              BorderRadius.circular(12),
                                        ),
                                        child: Text(
                                          status,
                                          style: const TextStyle(
                                            color: Colors.white,
                                            fontSize: 12,
                                          ),
                                        ),
                                      ),
                                    ],
                                  ),
                                  const SizedBox(height: 8),
                                  Text(
                                      "Khách hàng: ${order["customerName"] ?? ""}"),
                                  Text("Email: ${order["email"] ?? ""}"),
                                  Text(
                                      "Địa chỉ: ${order["shippingAddress"] ?? ""}"),
                                  const SizedBox(height: 6),
                                  Text(
                                    "Ngày đặt: ${order["createdAt"] ?? ""}",
                                    style: const TextStyle(fontSize: 12),
                                  ),
                                  const SizedBox(height: 6),
                                  Text(
                                    "Tổng tiền: ${order["totalAmount"] ?? 0} đ",
                                    style: const TextStyle(
                                      fontWeight: FontWeight.bold,
                                      color: Colors.red,
                                    ),
                                  ),
                                  if (canCancel)
                                    Align(
                                      alignment: Alignment.centerRight,
                                      child: TextButton.icon(
                                        icon: const Icon(Icons.cancel,
                                            color: Colors.red),
                                        label: const Text(
                                          "Huỷ đơn",
                                          style: TextStyle(color: Colors.red),
                                        ),
                                        onPressed: () =>
                                            _confirmCancel(order["id"]),
                                      ),
                                    ),
                                ],
                              ),
                            ),
                          ),
                        );
                      },
                    ),
    );
  }
}
