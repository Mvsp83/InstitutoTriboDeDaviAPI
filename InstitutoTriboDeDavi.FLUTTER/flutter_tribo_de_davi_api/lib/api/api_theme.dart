import 'package:flutter/material.dart';

class AppTheme {
  static const Color primaryColor = Color(0xFF003366);
  static const Color secondaryColor = Colors.red;
  static const Color backgroundColor = Color.fromARGB(255, 125, 169, 213);
  static const Color buttonTextColor = Colors.white;
  static const Color textColor = Colors.white;
  static const Color iconColor = Colors.white;
  static const Color borderColor = Colors.white;

  static const TextStyle headerTextStyle = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    color: Colors.black,
  );

  static const TextStyle bodyTextStyle = TextStyle(
    fontSize: 16,
    color: Colors.white,
  );

  static const TextStyle buttonTextStyle = TextStyle(
    fontSize: 18,
    fontWeight: FontWeight.bold,
    color: buttonTextColor,
  );

  static final BoxDecoration cardDecoration = BoxDecoration(
    color: Colors.white30,
    borderRadius: BorderRadius.circular(15),
    boxShadow: const [
      BoxShadow(
        color: primaryColor,
        blurRadius: 8,
        offset: Offset(2, 4),
      ),
    ],
  );

  static ButtonStyle elevatedButtonStyle = ElevatedButton.styleFrom(
    backgroundColor: primaryColor,
    padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 20),
    textStyle: buttonTextStyle,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(8),
      side: const BorderSide(
        color: borderColor,
        width: 3.0,
      ),
    ),
  );
}
