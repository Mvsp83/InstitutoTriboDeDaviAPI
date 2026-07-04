import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';

/// Card de ação dos menus (home, cadastros, consultas, aulas).
/// Substitui os _buildActionCard que estavam duplicados em 4 telas.
class ActionCard extends StatelessWidget {
  final String title;
  final IconData icon;
  final Color color;
  final VoidCallback onPressed;

  const ActionCard({
    super.key,
    required this.title,
    required this.icon,
    this.color = AppTheme.primaryColor,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onPressed,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
        decoration: AppTheme.cardDecoration.copyWith(
          color: color,
          border: Border.all(color: AppTheme.borderColor, width: 2),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 40, color: AppTheme.iconColor),
            const SizedBox(width: 15),
            Text(
              title,
              style: AppTheme.bodyTextStyle.copyWith(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
