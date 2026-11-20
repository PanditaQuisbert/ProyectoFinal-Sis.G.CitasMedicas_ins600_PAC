import 'package:flutter_test/flutter_test.dart';

import 'package:salud_gestion_movil/main.dart';

void main() {
  testWidgets('App builds smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(const SaludGestionApp());
    expect(find.byType(SaludGestionApp), findsOneWidget);
  });
}
