import 'package:shared_preferences/shared_preferences.dart';

class AuthStorage {
  static const _kToken = 'token';
  static const _kNombre = 'nombre';
  static const _kRol = 'rol';
  static const _kUsuario = 'usuario';

  static Future<void> saveSession({
    required String token,
    required String nombreCompleto,
    required String rol,
    required String usuario,
  }) async {
    final p = await SharedPreferences.getInstance();
    await p.setString(_kToken, token);
    await p.setString(_kNombre, nombreCompleto);
    await p.setString(_kRol, rol);
    await p.setString(_kUsuario, usuario);
  }

  static Future<String?> getToken() async {
    final p = await SharedPreferences.getInstance();
    return p.getString(_kToken);
  }

  static Future<Map<String, String>> getUser() async {
    final p = await SharedPreferences.getInstance();
    return {
      'nombre': p.getString(_kNombre) ?? '',
      'rol': p.getString(_kRol) ?? '',
      'usuario': p.getString(_kUsuario) ?? '',
    };
  }

  static Future<void> clear() async {
    final p = await SharedPreferences.getInstance();
    await p.remove(_kToken);
    await p.remove(_kNombre);
    await p.remove(_kRol);
    await p.remove(_kUsuario);
  }
}
