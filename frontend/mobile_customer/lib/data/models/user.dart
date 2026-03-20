class User {
  final int? id;
  final String? email;
  final String? fullName;
  final String? phone;

  User({
    this.id,
    this.email,
    this.fullName,
    this.phone,
  });
  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json["id"],
      email: json["email"],
      fullName: json["fullName"],
      phone: json["phone"],
    );
  }
  factory User.guest() {
    return User(
      fullName: "Khách hàng",
      email: "guest@shoppet.com",
      phone: "0123456789",
    );
  }
}
