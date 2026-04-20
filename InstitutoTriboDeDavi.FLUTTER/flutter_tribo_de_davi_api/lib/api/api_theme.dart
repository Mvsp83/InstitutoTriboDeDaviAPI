import 'package:flutter/material.dart';

class AppTheme {
  // ─── Paleta principal ────────────────────────────────────────────────────────
  static const Color primaryColor = Color(0xFF0A0A0A); // fundo principal
  static const Color surfaceColor =
      Color(0xFF141414); // superfície de campos/cards
  static const Color accentColor = Color(0xFFF5C518); // âmbar — acento e CTAs
  static const Color borderColor = Color(0xFFF5C518); // borda sutil
  static const Color textColor = Color(0xFFFFFFFF); // texto primário
  static const Color textMutedColor =
      Color(0xFF777777); // texto secundário/hints

  // Mantidos por compatibilidade com outras telas (podem ser migrados depois)
  static const Color secondaryColor = accentColor;
  static const Color backgroundColor = Colors.black;
  static const Color buttonTextColor = Color(0xFF0A0A0A); // preto sobre âmbar
  static const Color iconColor = accentColor;

  // ─── Tipografia ──────────────────────────────────────────────────────────────
  static const TextStyle headerTextStyle = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.w700,
    color: textColor,
    letterSpacing: 0.5,
  );

  static const TextStyle bodyTextStyle = TextStyle(
    fontSize: 14,
    color: textColor,
    height: 1.5,
  );

  static const TextStyle buttonTextStyle = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w700,
    color: buttonTextColor,
    letterSpacing: 1.5,
  );

  static const TextStyle accentTextStyle = TextStyle(
    fontSize: 13,
    color: accentColor,
    decoration: TextDecoration.underline,
    decorationColor: accentColor,
  );

  // ─── Decorações ──────────────────────────────────────────────────────────────
  static final BoxDecoration cardDecoration = BoxDecoration(
    color: surfaceColor,
    borderRadius: BorderRadius.circular(12),
    border: Border.all(color: borderColor, width: 0.5),
  );

  // ─── Botão primário (âmbar) ───────────────────────────────────────────────────
  static final ButtonStyle elevatedButtonStyle = ElevatedButton.styleFrom(
    backgroundColor: accentColor,
    disabledBackgroundColor: accentColor.withOpacity(0.4),
    foregroundColor: buttonTextColor,
    elevation: 0,
    padding: const EdgeInsets.symmetric(vertical: 16),
    textStyle: buttonTextStyle,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(10),
    ),
  );

  // ─── Botão secundário (contorno) ─────────────────────────────────────────────
  static final ButtonStyle outlinedButtonStyle = OutlinedButton.styleFrom(
    foregroundColor: textColor,
    side: const BorderSide(color: borderColor, width: 0.5),
    padding: const EdgeInsets.symmetric(vertical: 16),
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(10),
    ),
  );

  // ─── Campos de texto ─────────────────────────────────────────────────────────
  static InputDecoration inputDecoration({
    required String label,
    required IconData prefixIcon,
    Widget? suffixIcon,
  }) {
    return InputDecoration(
      labelText: label,
      labelStyle: const TextStyle(color: textMutedColor, fontSize: 14),
      prefixIcon: Icon(prefixIcon, color: accentColor, size: 20),
      suffixIcon: suffixIcon,
      filled: true,
      fillColor: surfaceColor,
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: const BorderSide(color: borderColor, width: 0.5),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: const BorderSide(color: accentColor, width: 1),
      ),
      errorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: const BorderSide(color: Colors.redAccent, width: 0.5),
      ),
      focusedErrorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: const BorderSide(color: Colors.redAccent, width: 1),
      ),
      contentPadding: const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
    );
  }

  // ─── AlertDialog ─────────────────────────────────────────────────────────────
  static final alertDialogTheme = AlertDialog(
    backgroundColor: surfaceColor,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(16),
      side: const BorderSide(color: borderColor),
    ),
  );
}
