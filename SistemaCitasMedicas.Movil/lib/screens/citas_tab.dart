import 'package:flutter/material.dart';
import '../services/api_service.dart';

class CitasTab extends StatefulWidget {
  const CitasTab({super.key});
  @override
  State<CitasTab> createState() => _CitasTabState();
}

class _CitasTabState extends State<CitasTab> {
  List<dynamic> items = [];
  bool loading = true;
  String? error;
  DateTime? filtroFecha;
  String? filtroEstado;

  static const estados = ['Programada', 'Atendida', 'Cancelada'];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      loading = true;
      error = null;
    });
    try {
      final q = <String, String>{};
      if (filtroFecha != null) {
        q['fecha'] = _fmtDate(filtroFecha!);
      }
      if (filtroEstado != null && filtroEstado!.isNotEmpty) {
        q['estado'] = filtroEstado!;
      }
      final list = await ApiService.getList('/api/citas', q.isEmpty ? null : q);
      if (mounted) {
        setState(() {
          items = list;
          loading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          error = e.toString();
          loading = false;
        });
      }
    }
  }

  static String _fmtDate(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

  Future<void> _openForm([Map<String, dynamic>? cita]) async {
    final ok = await Navigator.of(context).push<bool>(
      MaterialPageRoute(builder: (_) => CitaFormScreen(cita: cita)),
    );
    if (ok == true) _load();
  }

  Future<void> _cancelar(Map<String, dynamic> c) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Cancelar cita'),
        content: const Text('¿Desea cancelar esta cita? El horario quedará disponible.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('No')),
          FilledButton(onPressed: () => Navigator.pop(context, true), child: const Text('Sí, cancelar')),
        ],
      ),
    );
    if (confirm != true) return;
    try {
      final res = await ApiService.postEmpty('/api/citas/${c['id']}/cancelar');
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(res['message']?.toString() ?? 'Cita cancelada.')));
      _load();
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.toString()), backgroundColor: Colors.red));
    }
  }

  Future<void> _cambiarEstado(Map<String, dynamic> c) async {
    final actual = (c['estado'] ?? '').toString();
    final opciones = estados.where((e) => e != actual).toList();
    if (opciones.isEmpty) return;
    final nuevo = await showDialog<String>(
      context: context,
      builder: (_) => SimpleDialog(
        title: const Text('Cambiar estado'),
        children: opciones
            .map((e) => SimpleDialogOption(onPressed: () => Navigator.pop(context, e), child: Text(e)))
            .toList(),
      ),
    );
    if (nuevo == null) return;
    try {
      final res = await ApiService.postEmpty('/api/citas/${c['id']}/estado', {'estado': nuevo});
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(res['message']?.toString() ?? 'Estado actualizado.')));
      _load();
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.toString()), backgroundColor: Colors.red));
    }
  }

  void _showDetalle(Map<String, dynamic> c) {
    final paciente = c['paciente'] as Map? ?? {};
    final medico = c['medico'] as Map? ?? {};
    final estado = (c['estado'] ?? '').toString();
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (_) => Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Detalle de cita #${c['id']}', style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Text('Paciente: ${paciente['nombreCompleto'] ?? ''} (${paciente['documento'] ?? ''})'),
            Text('Médico: ${medico['nombreCompleto'] ?? ''}'),
            Text('Especialidad: ${medico['especialidad'] ?? ''}'),
            Text('Fecha/Hora: ${(c['fechaHora'] ?? '').toString().replaceFirst('T', ' ')}'),
            Text('Estado: $estado'),
            Text('Observaciones: ${(c['observaciones'] ?? '').toString().isEmpty ? '-' : c['observaciones']}'),
            const SizedBox(height: 12),
            Wrap(spacing: 8, runSpacing: 8, children: [
              if (estado != 'Cancelada')
                FilledButton.icon(onPressed: () { Navigator.pop(context); _openForm(c); }, icon: const Icon(Icons.edit), label: const Text('Modificar')),
              if (estado != 'Cancelada')
                OutlinedButton.icon(onPressed: () { Navigator.pop(context); _cancelar(c); }, icon: const Icon(Icons.cancel), label: const Text('Cancelar')),
              if (estado != 'Cancelada')
                OutlinedButton.icon(onPressed: () { Navigator.pop(context); _cambiarEstado(c); }, icon: const Icon(Icons.sync_alt), label: const Text('Cambiar estado')),
              TextButton(onPressed: () => Navigator.pop(context), child: const Text('Cerrar')),
            ]),
          ],
        ),
      ),
    );
  }

  Color _estadoColor(String e) {
    switch (e) {
      case 'Atendida':
        return Colors.green;
      case 'Cancelada':
        return Colors.red;
      default:
        return Colors.orange;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(8, 8, 8, 4),
          child: Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  icon: const Icon(Icons.date_range),
                  label: Text(filtroFecha == null ? 'Fecha: todas' : 'Fecha: ${_fmtDate(filtroFecha!)}'),
                  onPressed: () async {
                    final picked = await showDatePicker(
                      context: context,
                      initialDate: filtroFecha ?? DateTime.now(),
                      firstDate: DateTime(2020),
                      lastDate: DateTime(2035),
                    );
                    if (picked != null) {
                      setState(() => filtroFecha = picked);
                      _load();
                    }
                  },
                ),
              ),
              const SizedBox(width: 8),
              IconButton(
                tooltip: 'Quitar fecha',
                onPressed: filtroFecha == null ? null : () { setState(() => filtroFecha = null); _load(); },
                icon: const Icon(Icons.clear),
              ),
              FilledButton.icon(onPressed: () => _openForm(), icon: const Icon(Icons.add), label: const Text('Nueva')),
            ],
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8),
          child: DropdownButtonFormField<String>(
            key: ValueKey('estado-${filtroEstado ?? ''}'),
            initialValue: filtroEstado,
            decoration: const InputDecoration(labelText: 'Estado', border: OutlineInputBorder(), isDense: true),
            items: [
              const DropdownMenuItem(value: '', child: Text('Todos')),
              ...estados.map((e) => DropdownMenuItem(value: e, child: Text(e))),
            ],
            onChanged: (v) {
              setState(() => filtroEstado = (v == null || v.isEmpty) ? null : v);
              _load();
            },
          ),
        ),
        const SizedBox(height: 4),
        Expanded(
          child: loading
              ? const Center(child: CircularProgressIndicator())
              : error != null
                  ? Center(
                      child: Padding(
                        padding: const EdgeInsets.all(16),
                        child: Column(mainAxisSize: MainAxisSize.min, children: [
                          Text(error!, style: const TextStyle(color: Colors.red)),
                          const SizedBox(height: 8),
                          FilledButton(onPressed: _load, child: const Text('Reintentar')),
                        ]),
                      ),
                    )
                  : items.isEmpty
                      ? const Center(child: Text('No hay citas para los filtros seleccionados.'))
                      : RefreshIndicator(
                          onRefresh: _load,
                          child: ListView.builder(
                            itemCount: items.length,
                            itemBuilder: (_, i) {
                              final c = items[i] as Map<String, dynamic>;
                              final paciente = c['paciente'] as Map? ?? {};
                              final medico = c['medico'] as Map? ?? {};
                              final estado = (c['estado'] ?? '').toString();
                              return Card(
                                margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                child: ListTile(
                                  onTap: () => _showDetalle(c),
                                  leading: CircleAvatar(
                                    backgroundColor: _estadoColor(estado).withValues(alpha: 0.15),
                                    child: Icon(Icons.event, color: _estadoColor(estado)),
                                  ),
                                  title: Text(paciente['nombreCompleto']?.toString() ?? 'Paciente'),
                                  subtitle: Text(
                                    '${(c['fechaHora'] ?? '').toString().replaceFirst('T', ' ')}\n${medico['nombreCompleto'] ?? ''} · ${medico['especialidad'] ?? ''}',
                                  ),
                                  isThreeLine: true,
                                  trailing: Chip(
                                    label: Text(estado, style: const TextStyle(fontSize: 12)),
                                    backgroundColor: _estadoColor(estado).withValues(alpha: 0.15),
                                  ),
                                ),
                              );
                            },
                          ),
                        ),
        ),
      ],
    );
  }
}

class CitaFormScreen extends StatefulWidget {
  final Map<String, dynamic>? cita;
  const CitaFormScreen({super.key, this.cita});
  @override
  State<CitaFormScreen> createState() => _CitaFormScreenState();
}

class _CitaFormScreenState extends State<CitaFormScreen> {
  final _form = GlobalKey<FormState>();
  final _obsCtrl = TextEditingController();
  bool loadingInit = true;
  bool saving = false;
  String? error;

  List<dynamic> pacientes = [];
  List<dynamic> especialidades = [];
  List<dynamic> medicos = [];

  int? pacienteId;
  int? especialidadId;
  int? medicoId;
  DateTime fecha = DateTime.now().add(const Duration(days: 1));
  String? hora; // HH:mm
  List<String> ocupados = [];

  bool get isEdit => widget.cita != null;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    try {
      final results = await Future.wait([
        ApiService.getList('/api/pacientes', {'incluirInactivos': 'false'}),
        ApiService.getList('/api/catalogos/especialidades', {'soloActivas': 'true'}),
      ]);
      pacientes = results[0];
      especialidades = results[1];

      if (isEdit) {
        final c = widget.cita!;
        final p = c['paciente'] as Map? ?? {};
        final m = c['medico'] as Map? ?? {};
        pacienteId = (p['id'] as num?)?.toInt();
        especialidadId = (m['especialidadId'] as num?)?.toInt();
        medicoId = (m['id'] as num?)?.toInt();
        final fh = DateTime.tryParse((c['fechaHora'] ?? '').toString());
        if (fh != null) {
          fecha = DateTime(fh.year, fh.month, fh.day);
          hora = '${fh.hour.toString().padLeft(2, '0')}:${fh.minute.toString().padLeft(2, '0')}';
        }
        _obsCtrl.text = (c['observaciones'] ?? '').toString();
        if (especialidadId != null) {
          await _loadMedicos(especialidadId!);
        }
        if (medicoId != null) {
          await _loadDisponibilidad();
        }
      }
      if (mounted) setState(() => loadingInit = false);
    } catch (e) {
      if (mounted) setState(() { error = e.toString(); loadingInit = false; });
    }
  }

  Future<void> _loadMedicos(int espId) async {
    try {
      final list = await ApiService.getList('/api/catalogos/medicos', {
        'especialidadId': '$espId',
        'soloActivos': 'true',
      });
      if (mounted) setState(() => medicos = list);
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.toString()), backgroundColor: Colors.red));
      }
    }
  }

  Future<void> _loadDisponibilidad() async {
    if (medicoId == null) return;
    try {
      final q = <String, String>{
        'medicoId': '$medicoId',
        'fecha': _fmtDate(fecha),
      };
      if (isEdit && widget.cita?['id'] != null) {
        q['excluirCitaId'] = '${widget.cita!['id']}';
      }
      final res = await ApiService.getMap('/api/citas/disponibilidad', q);
      final list = (res['ocupados'] as List? ?? []).map((e) => e.toString()).toList();
      if (mounted) setState(() => ocupados = list);
    } catch (e) {
      if (mounted) setState(() => ocupados = []);
    }
  }

  static String _fmtDate(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

  List<String> _generarHoras() {
    final out = <String>[];
    for (int h = 7; h <= 19; h++) {
      for (final m in [0, 30]) {
        if (h == 19 && m > 0) continue;
        out.add('${h.toString().padLeft(2, '0')}:${m.toString().padLeft(2, '0')}');
      }
    }
    return out;
  }

  Future<void> _pickFecha() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: fecha,
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked != null) {
      setState(() => fecha = picked);
      await _loadDisponibilidad();
    }
  }

  Future<void> _save() async {
    if (!_form.currentState!.validate()) return;
    if (pacienteId == null || medicoId == null || hora == null) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Complete paciente, médico y hora.')));
      return;
    }
    final parts = hora!.split(':');
    final fechaHora = DateTime(fecha.year, fecha.month, fecha.day, int.parse(parts[0]), int.parse(parts[1]));
    final body = {
      'pacienteId': pacienteId,
      'medicoId': medicoId,
      'fechaHora': fechaHora.toIso8601String(),
      'observaciones': _obsCtrl.text.trim(),
    };

    // Validación previa de disponibilidad/reglas (opcional, backend también valida)
    try {
      final q = <String, String>{
        'medicoId': '$medicoId',
        'fechaHora': fechaHora.toIso8601String(),
        if (especialidadId != null) 'especialidadId': '$especialidadId',
        if (isEdit && widget.cita?['id'] != null) 'excluirCitaId': '${widget.cita!['id']}',
      };
      final val = await ApiService.getMap('/api/citas/validar', q);
      if (val['ok'] != true) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(val['message']?.toString() ?? 'Horario no disponible.'), backgroundColor: Colors.red));
        }
        return;
      }
    } catch (_) {
      // si falla la validación previa, intentamos guardar y el backend responderá
    }

    setState(() => saving = true);
    try {
      if (isEdit) {
        await ApiService.put('/api/citas/${widget.cita!['id']}', body);
      } else {
        await ApiService.post('/api/citas', body);
      }
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(isEdit ? 'Cita actualizada correctamente.' : 'Cita programada correctamente.')));
      Navigator.pop(context, true);
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.toString()), backgroundColor: Colors.red));
      }
    } finally {
      if (mounted) setState(() => saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(isEdit ? 'Modificar cita' : 'Programar cita'),
        backgroundColor: const Color(0xFF0F766E),
        foregroundColor: Colors.white,
      ),
      body: loadingInit
          ? const Center(child: CircularProgressIndicator())
          : error != null
              ? Center(child: Padding(padding: const EdgeInsets.all(16), child: Text(error!, style: const TextStyle(color: Colors.red))))
              : Form(
                  key: _form,
                  child: ListView(
                    padding: const EdgeInsets.all(16),
                    children: [
                      DropdownButtonFormField<int>(
                        key: ValueKey('pac-$pacienteId'),
                        initialValue: pacienteId,
                        isExpanded: true,
                        decoration: const InputDecoration(labelText: 'Paciente *', border: OutlineInputBorder()),
                        items: pacientes.map((p) {
                          final m = p as Map<String, dynamic>;
                          return DropdownMenuItem<int>(
                            value: (m['id'] as num).toInt(),
                            child: Text('${m['documento']} - ${m['nombres']} ${m['apellidos']}', overflow: TextOverflow.ellipsis),
                          );
                        }).toList(),
                        onChanged: (v) => setState(() => pacienteId = v),
                        validator: (v) => v == null ? 'Seleccione un paciente' : null,
                      ),
                      const SizedBox(height: 12),
                      DropdownButtonFormField<int>(
                        key: ValueKey('esp-$especialidadId'),
                        initialValue: especialidadId,
                        isExpanded: true,
                        decoration: const InputDecoration(labelText: 'Especialidad *', border: OutlineInputBorder()),
                        items: especialidades.map((e) {
                          final m = e as Map<String, dynamic>;
                          return DropdownMenuItem<int>(
                            value: (m['id'] as num).toInt(),
                            child: Text(m['nombre']?.toString() ?? ''),
                          );
                        }).toList(),
                        onChanged: (v) async {
                          setState(() {
                            especialidadId = v;
                            medicoId = null;
                            medicos = [];
                            ocupados = [];
                          });
                          if (v != null) await _loadMedicos(v);
                        },
                        validator: (v) => v == null ? 'Seleccione una especialidad' : null,
                      ),
                      const SizedBox(height: 12),
                      DropdownButtonFormField<int>(
                        key: ValueKey('med-$medicoId'),
                        initialValue: medicoId,
                        isExpanded: true,
                        decoration: const InputDecoration(labelText: 'Médico *', border: OutlineInputBorder()),
                        items: medicos.map((m) {
                          final x = m as Map<String, dynamic>;
                          return DropdownMenuItem<int>(
                            value: (x['id'] as num).toInt(),
                            child: Text(x['nombreCompleto']?.toString() ?? ''),
                          );
                        }).toList(),
                        onChanged: (v) async {
                          setState(() {
                            medicoId = v;
                            hora = null;
                          });
                          await _loadDisponibilidad();
                        },
                        validator: (v) => v == null ? 'Seleccione un médico' : null,
                      ),
                      const SizedBox(height: 12),
                      ListTile(
                        contentPadding: EdgeInsets.zero,
                        title: const Text('Fecha *'),
                        subtitle: Text(_fmtDate(fecha)),
                        trailing: OutlinedButton.icon(onPressed: _pickFecha, icon: const Icon(Icons.calendar_today), label: const Text('Elegir')),
                      ),
                      const SizedBox(height: 8),
                      if (medicoId != null) ...[
                        const Text('Hora disponible *', style: TextStyle(fontWeight: FontWeight.w600)),
                        const SizedBox(height: 6),
                        Wrap(
                          spacing: 8,
                          runSpacing: 8,
                          children: _generarHoras().map((h) {
                            final ocupado = ocupados.contains(h);
                            final selected = hora == h;
                            return ChoiceChip(
                              label: Text(h),
                              selected: selected,
                              onSelected: ocupado ? null : (v) => setState(() => hora = v ? h : null),
                              avatar: ocupado ? const Icon(Icons.block, size: 16) : null,
                              selectedColor: const Color(0xFF0F766E),
                              labelStyle: TextStyle(
                                color: ocupado ? Colors.grey : (selected ? Colors.white : null),
                                decoration: ocupado ? TextDecoration.lineThrough : null,
                              ),
                            );
                          }).toList(),
                        ),
                        const SizedBox(height: 4),
                        Text('Horarios ocupados no disponibles: ${ocupados.isEmpty ? "ninguno" : ocupados.join(", ")}',
                            style: const TextStyle(fontSize: 12)),
                        const SizedBox(height: 12),
                      ] else
                        const Padding(
                          padding: EdgeInsets.only(bottom: 12),
                          child: Text('Seleccione médico y fecha para ver horarios disponibles.', style: TextStyle(fontSize: 12)),
                        ),
                      TextFormField(
                        controller: _obsCtrl,
                        decoration: const InputDecoration(labelText: 'Observaciones', border: OutlineInputBorder()),
                        maxLines: 3,
                      ),
                      const SizedBox(height: 20),
                      FilledButton.icon(
                        onPressed: saving ? null : _save,
                        icon: saving ? const SizedBox(width: 16, height: 16, child: CircularProgressIndicator(strokeWidth: 2)) : const Icon(Icons.save),
                        label: Text(isEdit ? 'Guardar cambios' : 'Programar cita'),
                      ),
                      const SizedBox(height: 8),
                      const Text('La disponibilidad y las reglas (RN-04, duplicados, estados) se validan en el servidor.'),
                    ],
                  ),
                ),
    );
  }
}
