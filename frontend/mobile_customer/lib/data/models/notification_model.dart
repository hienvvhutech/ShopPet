class NotificationModel {
  final int id;
  final String title;
  final String content;
  final bool isRead;
  final DateTime createdAt;
  final int? orderId;

  NotificationModel({
    required this.id,
    required this.title,
    required this.content,
    required this.isRead,
    required this.createdAt,
    this.orderId,
  });
  factory NotificationModel.fromJson(Map<String, dynamic> json) {
    return NotificationModel(
      id: json['id'],
      title: json['title'],
      content: json['content'],
      isRead: json['isRead'] ?? false,
      createdAt: DateTime.parse(json['createdAt']),
      orderId: json['orderId'],
    );
  }
}
