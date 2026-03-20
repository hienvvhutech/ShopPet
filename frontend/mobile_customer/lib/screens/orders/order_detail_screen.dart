import 'package:flutter/material.dart';
import 'package:mobile_customer/data/repositories/order_repository.dart';

class OrderDetailScreen extends StatefulWidget {
  final int orderId;
  const OrderDetailScreen({super.key, required this.orderId});
  @override
  State<OrderDetailScreen> createState() => _OrderDetailScreenState();
}

class _OrderDetailScreenState extends State<OrderDetailScreen> {
  final OrderRepository _repo = OrderRepository();
  bool isLoading = true;
  String? error;
  Map<String, dynamic>? order;
  @override
  void initState() {
    super.initState();
    _loadOrder();
  }

  Future<void> _loadOrder() async {
    try {
      final res = await _repo.getOrderDetail(widget.orderId);
      setState(() => order = res);
    } catch (e) {
      setState(() => error = e.toString());
    } finally {
      setState(() => isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Đơn hàng #${widget.orderId}"),
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : error != null
              ? Center(child: Text(error!))
              : _buildContent(),
    );
  }

  Widget _buildContent() {
    final data = order ?? {};
    final List items = (data['orderItems'] as List?) ?? [];

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _info("Khách hàng", data["customerName"]),
          _info("Email", data["email"]),
          _info("Địa chỉ", data["shippingAddress"]),
          _info("Trạng thái", data["status"]),
          _info("Ngày đặt", data["createdAt"]),
          const Divider(height: 32),
          const Text(
            "Sản phẩm đã đặt",
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 8),
          if (items.isEmpty)
            const Text("Không có sản phẩm")
          else
            ...items.map((item) => Card(
                  child: ListTile(
                    title: Text(item["productName"]?.toString() ?? ""),
                    subtitle: Text(
                      "Số lượng: ${item["quantity"] ?? 0}",
                    ),
                    trailing: Text(
                      "${item["unitPrice"] ?? 0} đ",
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ),
                )),
          const Divider(height: 32),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              "Tổng tiền: ${data["totalAmount"] ?? 0} đ",
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.red,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _info(String label, dynamic value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 110,
            child: Text(
              "$label:",
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
          ),
          Expanded(
            child: Text(value?.toString() ?? ""),
          ),
        ],
      ),
    );
  }
}
