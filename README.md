# Assetto – TeleportPlugin

Plugin pentru **AssettoServer** (compujuckel/AssettoServer) care permite jucătorilor să se teleporteze unul la altul prin chat.

---

## Comandă

```
/tp <nume_jucator>
```

- Acceptă **nume parțial**, case-insensitive (ex. `/tp Ion` va găsi `Ionescu`).
- Dacă există mai mulți jucători cu același fragment de nume, este selectat primul care se potrivește exact; altfel primul potrivire parțială.

---

## Structura proiectului

```
TeleportPlugin/
├── TeleportPlugin.csproj       # Proiect C# (.NET 8)
├── TeleportPlugin.cs           # Logica principală + handler chat
├── TeleportConfiguration.cs    # Configurare (YAML)
├── TeleportModule.cs           # Înregistrare DI în AssettoServer
└── cfg/
    └── teleport_plugin.yml     # Fișier de configurare (copie în cfg/ server)
```

---

## Instalare

### 1. Compilare

```bash
# Setează calea corectă spre AssettoServer.dll în TeleportPlugin.csproj
dotnet build TeleportPlugin/TeleportPlugin.csproj -c Release
```

### 2. Copiere DLL

Copiază `TeleportPlugin.dll` (din `bin/Release/net8.0/`) în folderul `plugins/` al serverului AssettoServer.

### 3. Configurare

Copiază `TeleportPlugin/cfg/teleport_plugin.yml` în folderul `cfg/` al serverului.

Opțiuni disponibile:

| Opțiune              | Tip     | Default | Descriere                                          |
|----------------------|---------|---------|----------------------------------------------------|
| `SpawnOffsetMeters`  | `float` | `3.0`   | Offset X (metri) față de mașina țintă              |
| `NotifyTarget`       | `bool`  | `true`  | Trimite notificare chat jucătorului destinație      |

### 4. Activare în `extra_cfg.yml`

Adaugă în `extra_cfg.yml` al serverului:

```yaml
EnablePlugins:
  - TeleportPlugin
```

---

## Cerințe

| Componentă         | Versiune minimă |
|--------------------|-----------------|
| AssettoServer      | 0.0.52+         |
| .NET SDK           | 8.0             |
| Assetto Corsa      | 1.16+           |

> **Notă privind teleportarea:** AssettoServer suportă repoziționarea autoritativă a mașinilor prin `EntryCar.SetPosition()`. Această metodă funcționează optim cu **Custom Shaders Patch (CSP)** instalat pe clienți. Fără CSP, mașina se va muta server-side, dar clientul poate reseta poziția după câteva frame-uri.

---

## Exemple

```
# Teleport la un jucător cu numele exact
/tp Ionescu

# Teleport folosind nume parțial
/tp Ion

# Nu funcționează – același jucător
/tp <propriul_tau_nume>
```

---

## Licență

MIT
