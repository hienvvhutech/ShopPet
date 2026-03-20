class CartItem {
  final int productId;
  final String name;
  final int price;
  final String imageUrl;
  int quantity;

  CartItem({
    required this.productId,
    required this.name,
    required this.price,
    required this.imageUrl,
    this.quantity = 1,
  });
  factory CartItem.fromJson(Map<String, dynamic> json) {
    return CartItem(
      productId: json['productId'],
      name: json['name'],
      price: json['price'],
      imageUrl: json['image'],
      quantity: json['quantity'],
    );
  }
  Map<String, dynamic> toJson() {
    return {
      "productId": productId,
      "name": name,
      "price": price,
      "image": imageUrl,
      "quantity": quantity,
    };
  }

  int get totalPrice => price * quantity;
}
