import 'package:flutter/material.dart';
import '../services/api_service.dart';

class DashboardTab extends StatefulWidget {
  const DashboardTab({super.key});
  @override
  State<DashboardTab> createState() => _DashboardTabState();
}

class _DashboardTabState extends State<DashboardTab> {
  Map<String, dynamic>? data;
  String? error;
  bool loading = true;

  @override
  void initState() { super.initState(); _load(); }

  Future<void> _load() async {
    setState(() { loading = true; error = null; });
    try {
      final d = await ApiService.getMap('/api/dashboard');
      if (mounted) setState(() { data = d; loading = false; });
    } catch (e) {
      if (mounted) setState(() { error = e.toString(); loading = false; });
    }
  }

  Widget _card(String title, String value, Color color) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
          Text(title, style: const TextStyle(fontSize: 13)),
          const SizedBox(height: 6),
          Text(value, style: TextStyle(fontSize: 26, fontWeight: FontWeight.bold, color: color)),
        ]),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (loading) return const Center(child: CircularProgressIndicator());
    if (error != null) return Center(child: Padding(padding: const EdgeInsets.all(16), child: Column(mainAxisSize: MainAxisSize.min, children: [Text(error!, style: const TextStyle(color: Colors.red)), const SizedBox(height: 8), FilledButton(onPressed: _load, child: const Text('Reintentar'))])));
    final d = data!;
    final proximas = (d['proximasCitas'] as List? ?? []);
    final ultimas = (d['ultimasCitas'] as List? ?? []);
    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        padding: const EdgeInsets.all(12),
        children: [
          const Text('Panel principal', style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
          const SizedBox(height: 4),
          const Text('Indicadores obtenidos en tiempo real desde el backend.'),
          const SizedBox(height: 12),
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            childAspectRatio: 1.7,
            children: [
              _card('Pacientes activos', '${d['pacientesActivos']}', Colors.teal),
              _card('Médicos activos', '${d['medicosActivos']}', Colors.blue),
              _card('Especialidades', '${d['especialidadesActivas']}', Colors.indigo),
              _card('Programadas hoy', '${d['citasProgramadasHoy']}', Colors.orange),
              _card('Atendidas', '${d['citasAtendidas']}', Colors.green),
              _card('Canceladas', '${d['citasCanceladas']}', Colors.red),
            ],
          ),
          const SizedBox(height: 12),
          const Text('Próximas citas programadas', style: TextStyle(fontWeight: FontWeight.bold)),
          if (proximas.isEmpty) const Padding(padding: EdgeInsets.all(8), child: Text('No hay próximas citas.')),
          ...proximas.map((c) => ListTile(
            leading: const Icon(Icons.event_available),
            title: Text(c['paciente'] ?? ''),
            subtitle: Text('${(c['fechaHora'] ?? '').toString().replaceFirst('T', ' ')} · ${c['medico'] ?? ''}'),
          )),
          const SizedBox(height: 8),
          const Text('Últimas citas registradas', style: TextStyle(fontWeight: FontWeight.bold)),
          if (ultimas.isEmpty) const Padding(padding: EdgeInsets.all(8), child: Text('No hay citas registradas.')),
          ...ultimas.map((c) => ListTile(
            leading: const Icon(Icons.history),
            title: Text(c['paciente'] ?? ''),
            subtitle: Text('${(c['fechaHora'] ?? '').toString().replaceFirst('T', ' ')} · ${c['estado'] ?? ''}'),
          )),
        ],
      ),
    );
  }
}
