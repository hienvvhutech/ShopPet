class Product {
  final int id;
  final String name;
  final String? description;
  final int price;
  final String imageUrl;
  final int? categoryId;
  final int stock;

  Product({
    required this.id,
    required this.name,
    required this.price,
    required this.imageUrl,
    required this.stock,
    this.description,
    this.categoryId,
  });
  factory Product.fromJson(Map<String, dynamic> json) {
    return Product(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      price: (json['price'] as num).toInt(),
      stock: json['stockQuantity'] ?? 0,
      categoryId: json['categoryId'],
      imageUrl: json['imageUrl'] != null && json['imageUrl'] != ""
          ? "http://10.0.2.2:5287${json['imageUrl']}"
          : "",
    );
  }

  Map<String, dynamic> toJson() {
    return {
      "id": id,
      "name": name,
      "description": description,
      "price": price,
      "image": imageUrl,
      "stock": stock,
      "categoryId": categoryId,
    };
  }
}
