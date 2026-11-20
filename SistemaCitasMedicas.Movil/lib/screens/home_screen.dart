import 'package:flutter/material.dart';
import '../services/auth_storage.dart';
import 'login_screen.dart';
import 'dashboard_tab.dart';
import 'pacientes_tab.dart';
import 'citas_tab.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});
  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _index = 0;
  Map<String, String> _user = {'nombre': '', 'rol': '', 'usuario': ''};

  @override
  void initState() {
    super.initState();
    _loadUser();
  }

  Future<void> _loadUser() async {
    final u = await AuthStorage.getUser();
    if (mounted) setState(() => _user = u);
  }

  Future<void> _logout() async {
    await AuthStorage.clear();
    if (!mounted) return;
    Navigator.of(context).pushAndRemoveUntil(MaterialPageRoute(builder: (_) => const LoginScreen()), (_) => false);
  }

  @override
  Widget build(BuildContext context) {
    final tabs = [
      const DashboardTab(),
      const PacientesTab(),
      const CitasTab(),
    ];
    return Scaffold(
      appBar: AppBar(
        title: const Text('SALUD GESTIÓN'),
        backgroundColor: const Color(0xFF0F766E),
        foregroundColor: Colors.white,
        actions: [
          IconButton(icon: const Icon(Icons.logout), tooltip: 'Cerrar sesión', onPressed: _logout),
        ],
      ),
      drawer: Drawer(
        child: ListView(
          children: [
            UserAccountsDrawerHeader(
              decoration: const BoxDecoration(color: Color(0xFF0F766E)),
              accountName: Text(_user['nombre'] ?? ''),
              accountEmail: Text('${_user['usuario']} (${_user['rol']})'),
              currentAccountPicture: const CircleAvatar(child: Icon(Icons.person)),
            ),
            ListTile(leading: const Icon(Icons.dashboard), title: const Text('Dashboard'), onTap: () { setState(()=>_index=0); Navigator.pop(context); }),
            ListTile(leading: const Icon(Icons.people), title: const Text('Pacientes'), onTap: () { setState(()=>_index=1); Navigator.pop(context); }),
            ListTile(leading: const Icon(Icons.event), title: const Text('Citas médicas'), onTap: () { setState(()=>_index=2); Navigator.pop(context); }),
            const Divider(),
            ListTile(leading: const Icon(Icons.logout), title: const Text('Cerrar sesión'), onTap: _logout),
          ],
        ),
      ),
      body: IndexedStack(index: _index, children: tabs),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (i) => setState(() => _index = i),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.dashboard_outlined), selectedIcon: Icon(Icons.dashboard), label: 'Dashboard'),
          NavigationDestination(icon: Icon(Icons.people_outline), selectedIcon: Icon(Icons.people), label: 'Pacientes'),
          NavigationDestination(icon: Icon(Icons.event_outlined), selectedIcon: Icon(Icons.event), label: 'Citas'),
        ],
      ),
    );
  }
}
