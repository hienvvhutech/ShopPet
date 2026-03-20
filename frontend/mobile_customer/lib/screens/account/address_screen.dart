import 'package:flutter/material.dart';
import 'package:mobile_customer/core/api/api_service.dart';
import 'package:mobile_customer/core/storage/local_storage.dart';

class AddressScreen extends StatefulWidget {
  const AddressScreen();
  @override
  State<AddressScreen> createState() => _AddressScreenState();
}

class _AddressScreenState extends State<AddressScreen> {
  final TextEditingController _ctrl = TextEditingController();
  final ApiService _api = ApiService();
  List<Map<String, dynamic>> addresses = [];
  int? defaultIndex;
  @override
  void initState() {
    super.initState();
    _loadFromServer();
  }

  Future<void> _loadFromServer() async {
    final res = await _api.get("Addresses");
    setState(() {
      addresses = List<Map<String, dynamic>>.from(res);
      defaultIndex = addresses.indexWhere((e) => e["isDefault"] == true);
    });
    await LocalStorage.cacheAddresses(
      addresses.map((e) => e["addressLine"].toString()).toList(),
    );
    if (defaultIndex != -1) {
      await LocalStorage.setDefaultIndex(defaultIndex!);
    }
  }

  Future<void> _addAddress() async {
    if (_ctrl.text.trim().isEmpty) return;

    await _api.post(
      "Addresses",
      {
        "name": "Nhà",
        "phone": "0000000000",
        "addressLine": _ctrl.text.trim(),
        "isDefault": true,
      },
    );
    _ctrl.clear();
    await _loadFromServer();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Địa chỉ giao hàng")),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              controller: _ctrl,
              decoration: const InputDecoration(
                labelText: "Thêm địa chỉ mới",
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 8),
            ElevatedButton(
              onPressed: _addAddress,
              child: const Text("Thêm địa chỉ"),
            ),
            const Divider(),
            Expanded(
              child: ListView.builder(
                itemCount: addresses.length,
                itemBuilder: (_, i) {
                  final a = addresses[i];
                  return ListTile(
                    leading: Icon(
                      a["isDefault"]
                          ? Icons.check_circle
                          : Icons.circle_outlined,
                      color: a["isDefault"] ? Colors.green : null,
                    ),
                    title: Text(a["addressLine"]),
                    subtitle: a["isDefault"] ? const Text("Mặc định") : null,
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}
