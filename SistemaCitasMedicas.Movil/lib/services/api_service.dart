import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config.dart';
import 'auth_storage.dart';

class ApiException implements Exception {
  final String message;
  final int status;
  ApiException(this.message, this.status);
  @override
  String toString() => message;
}

class ApiService {
  static Future<Map<String, String>> _headers() async {
    final token = await AuthStorage.getToken();
    return {
      'Content-Type': 'application/json',
      if (token != null && token.isNotEmpty) 'Authorization': 'Bearer $token',
    };
  }

  static Uri _uri(String path, [Map<String, String>? query]) {
    final base = Uri.parse(AppConfig.apiBaseUrl);
    return Uri(
      scheme: base.scheme,
      host: base.host,
      port: base.port,
      path: path,
      queryParameters: (query == null || query.isEmpty) ? null : query,
    );
  }

  static dynamic _handle(http.Response res) {
    final body = res.body.isNotEmpty ? jsonDecode(res.body) : null;
    if (res.statusCode >= 200 && res.statusCode < 300) return body;
    String msg = 'Error (${res.statusCode})';
    if (body is Map && body['message'] != null) msg = body['message'].toString();
    throw ApiException(msg, res.statusCode);
  }

  static Future<Map<String, dynamic>> login(String usuario, String contrasena) async {
    final res = await http.post(
      _uri('/api/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'nombreUsuario': usuario, 'contrasena': contrasena}),
    );
    final data = _handle(res) as Map<String, dynamic>;
    final u = data['usuario'] as Map<String, dynamic>;
    await AuthStorage.saveSession(
      token: data['token'] as String,
      nombreCompleto: (u['nombreCompleto'] ?? '').toString(),
      rol: (u['rol'] ?? '').toString(),
      usuario: (u['nombreUsuario'] ?? '').toString(),
    );
    return data;
  }

  static Future<List<dynamic>> getList(String path, [Map<String, String>? query]) async {
    final res = await http.get(_uri(path, query), headers: await _headers());
    final data = _handle(res);
    return data is List ? data : [];
  }

  static Future<Map<String, dynamic>> getMap(String path, [Map<String, String>? query]) async {
    final res = await http.get(_uri(path, query), headers: await _headers());
    return (_handle(res) as Map).cast<String, dynamic>();
  }

  static Future<Map<String, dynamic>> post(String path, Map<String, dynamic> body) async {
    final res = await http.post(_uri(path), headers: await _headers(), body: jsonEncode(body));
    final data = _handle(res);
    return data is Map ? data.cast<String, dynamic>() : {'ok': true};
  }

  static Future<Map<String, dynamic>> put(String path, Map<String, dynamic> body) async {
    final res = await http.put(_uri(path), headers: await _headers(), body: jsonEncode(body));
    final data = _handle(res);
    return data is Map ? data.cast<String, dynamic>() : {'ok': true};
  }

  static Future<Map<String, dynamic>> postEmpty(String path, [Map<String, String>? query]) async {
    final res = await http.post(_uri(path, query), headers: await _headers());
    final data = _handle(res);
    return data is Map ? data.cast<String, dynamic>() : {'ok': true};
  }
}
