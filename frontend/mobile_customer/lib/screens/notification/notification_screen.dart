import 'package:flutter/material.dart';
import 'package:mobile_customer/providers/notification_provider.dart';
import 'package:mobile_customer/screens/orders/order_detail_screen.dart';
import 'package:provider/provider.dart';

class NotificationScreen extends StatefulWidget {
  const NotificationScreen();

  @override
  State<NotificationScreen> createState() => _NotificationScreenState();
}

class _NotificationScreenState extends State<NotificationScreen> {
  @override
  void initState() {
    super.initState();
    context.read<NotificationProvider>().loadNotifications();
  }

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<NotificationProvider>();

    return Scaffold(
      appBar: AppBar(
        title: const Text("Thông báo"),
      ),
      body: provider.isLoading
          ? const Center(child: CircularProgressIndicator())
          : provider.notifications.isEmpty
              ? const Center(child: Text("Không có thông báo"))
              : ListView.builder(
                  itemCount: provider.notifications.length,
                  itemBuilder: (_, index) {
                    final noti = provider.notifications[index];

                    return ListTile(
                      leading: Icon(
                        noti.isRead
                            ? Icons.notifications
                            : Icons.notifications_active,
                        color: noti.isRead ? Colors.grey : Colors.deepPurple,
                      ),
                      title: Text(
                        noti.title,
                        style: TextStyle(
                          fontWeight:
                              noti.isRead ? FontWeight.normal : FontWeight.bold,
                        ),
                      ),
                      subtitle: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(noti.content),
                          const SizedBox(height: 4),
                          Text(
                            noti.createdAt.toLocal().toString(),
                            style: const TextStyle(
                              fontSize: 11,
                              color: Colors.grey,
                            ),
                          ),
                        ],
                      ),
                      onTap: () {
                        debugPrint(
                            " CLICK NOTI id=${noti.id}, orderId=${noti.orderId}");
                        if (noti.orderId != null) {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => OrderDetailScreen(
                                orderId: noti.orderId!,
                              ),
                            ),
                          );
                        }
                      },
                    );
                  },
                ),
    );
  }
}
