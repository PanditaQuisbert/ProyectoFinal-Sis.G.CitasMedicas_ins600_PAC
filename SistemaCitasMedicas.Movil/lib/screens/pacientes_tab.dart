import 'package:flutter/material.dart';
import '../services/api_service.dart';

class PacientesTab extends StatefulWidget {
  const PacientesTab({super.key});
  @override
  State<PacientesTab> createState() => _PacientesTabState();
}

class _PacientesTabState extends State<PacientesTab> {
  List<dynamic> items = [];
  bool loading = true;
  String? error;
  String buscar = '';

  @override
  void initState() { super.initState(); _load(); }

  Future<void> _load() async {
    setState(() { loading = true; error = null; });
    try {
      final q = buscar.isEmpty ? null : {'buscar': buscar};
      final list = await ApiService.getList('/api/pacientes', q);
      if (mounted) setState(() { items = list; loading = false; });
    } catch (e) {
      if (mounted) setState(() { error = e.toString(); loading = false; });
    }
  }

  Future<void> _openForm([Map<String, dynamic>? p]) async {
    final ok = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      builder: (_) => PacienteForm(paciente: p),
    );
    if (ok == true) _load();
  }

  @override
  Widget build(BuildContext context) {
    return Column(children: [
      Padding(
        padding: const EdgeInsets.all(8),
        child: Row(children: [
          Expanded(child: TextField(decoration: const InputDecoration(prefixIcon: Icon(Icons.search), hintText: 'Buscar por documento/nombre', border: OutlineInputBorder()), onSubmitted: (v){ buscar=v; _load(); })),
          const SizedBox(width: 8),
          FilledButton.icon(onPressed: () => _openForm(), icon: const Icon(Icons.add), label: const Text('Nuevo')),
        ]),
      ),
      Expanded(
        child: loading
          ? const Center(child: CircularProgressIndicator())
          : error != null
            ? Center(child: Text(error!, style: const TextStyle(color: Colors.red)))
            : items.isEmpty
              ? const Center(child: Text('No hay pacientes.'))
              : RefreshIndicator(
                  onRefresh: _load,
                  child: ListView.builder(
                    itemCount: items.length,
                    itemBuilder: (_, i) {
                      final p = items[i] as Map<String, dynamic>;
                      final activo = p['activo'] == true;
                      return Card(
                        margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        child: ListTile(
                          title: Text('${p['nombres']} ${p['apellidos']}'),
                          subtitle: Text('Doc: ${p['documento']} · Tel: ${p['telefono'] ?? ''}\n${activo ? 'Activo' : 'Inactivo'}'),
                          isThreeLine: true,
                          trailing: IconButton(icon: const Icon(Icons.edit), onPressed: () => _openForm(p)),
                        ),
                      );
                    },
                  ),
                ),
      ),
    ]);
  }
}

class PacienteForm extends StatefulWidget {
  final Map<String, dynamic>? paciente;
  const PacienteForm({super.key, this.paciente});
  @override
  State<PacienteForm> createState() => _PacienteFormState();
}

class _PacienteFormState extends State<PacienteForm> {
  final _form = GlobalKey<FormState>();
  late final TextEditingController doc, nom, ape, tel, nac;
  bool activo = true;
  bool saving = false;

  @override
  void initState() {
    super.initState();
    final p = widget.paciente;
    doc = TextEditingController(text: p?['documento'] ?? '');
    nom = TextEditingController(text: p?['nombres'] ?? '');
    ape = TextEditingController(text: p?['apellidos'] ?? '');
    tel = TextEditingController(text: p?['telefono'] ?? '');
    nac = TextEditingController(text: p?['fechaNacimiento'] ?? '');
    activo = p?['activo'] ?? true;
  }

  Future<void> _save() async {
    if (!_form.currentState!.validate()) return;
    setState(() => saving = true);
    try {
      final body = {
        'documento': doc.text.trim(),
        'nombres': nom.text.trim(),
        'apellidos': ape.text.trim(),
        'telefono': tel.text.trim(),
        'fechaNacimiento': nac.text.trim().isEmpty ? null : nac.text.trim(),
        'activo': activo,
      };
      if (widget.paciente == null) {
        await ApiService.post('/api/pacientes', body);
      } else {
        await ApiService.put('/api/pacientes/${widget.paciente!['id']}', body);
      }
      if (!mounted) return;
      Navigator.pop(context, true);
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Paciente guardado correctamente.')));
    } catch (e) {
      if (mounted) ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.toString()), backgroundColor: Colors.red));
    } finally {
      if (mounted) setState(() => saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final bottom = MediaQuery.of(context).viewInsets.bottom;
    return Padding(
      padding: EdgeInsets.only(left: 16, right: 16, top: 16, bottom: bottom + 16),
      child: Form(
        key: _form,
        child: SingleChildScrollView(
          child: Column(mainAxisSize: MainAxisSize.min, children: [
            Text(widget.paciente == null ? 'Registrar paciente' : 'Editar paciente', style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            TextFormField(controller: doc, decoration: const InputDecoration(labelText: 'Documento *'), validator: (v)=>v==null||v.trim().isEmpty?'Requerido':null),
            TextFormField(controller: nom, decoration: const InputDecoration(labelText: 'Nombres *'), validator: (v)=>v==null||v.trim().isEmpty?'Requerido':null),
            TextFormField(controller: ape, decoration: const InputDecoration(labelText: 'Apellidos *'), validator: (v)=>v==null||v.trim().isEmpty?'Requerido':null),
            TextFormField(controller: tel, decoration: const InputDecoration(labelText: 'Teléfono *'), validator: (v)=>v==null||v.trim().isEmpty?'Requerido':null),
            TextFormField(controller: nac, decoration: const InputDecoration(labelText: 'Fecha nacimiento (YYYY-MM-DD)')),
            SwitchListTile(value: activo, onChanged: (v)=>setState(()=>activo=v), title: const Text('Activo')),
            const SizedBox(height: 8),
            FilledButton(onPressed: saving ? null : _save, child: saving ? const CircularProgressIndicator() : const Text('Guardar')),
          ]),
        ),
      ),
    );
  }
}
