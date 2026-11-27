# Handoff — Sebastián → Andrés

**Proyecto:** Taller1Unity · **Rama:** `feature/taller-final` · **Entrega:** martes 1 de diciembre de 2026
**Actualizado:** jueves 26 de noviembre de 2026

---

## Respuesta corta

**Sí, avancé.** El **100 % del código del juego está escrito, compila limpio y está pusheado**. Lo que falta es del lado de assets, y es tu parte.

Detalle completo en `Plan_Trabajo_Taller_Final.md` (es largo; este documento es el resumen accionable).

---

## 1. Qué entregué (código)

12 scripts nuevos en `Assets/Scripts/`, más un script de Editor que automatiza el setup:

| Script | Qué hace |
|---|---|
| `Player/PlayerController.cs` | Movimiento con `CharacterController` (en vez de la esfera con Rigidbody del avance 1). Correr con Shift, saltar con Espacio, frenado instantáneo, movimiento relativo a la cámara. Detecta golpes con muros vía `OnControllerColliderHit`. |
| `Player/GroundChecker.cs` | Detección de suelo por `SphereCast`, para que el salto solo funcione apoyado. |
| `Player/PlayerAnimationDriver.cs` | Alimenta los parámetros `Speed`, `Grounded` y `Jump` del Animator. |
| `Camera/CameraOrbit.cs` | Cámara en tercera persona con órbita de mouse y suavizado exponencial. |
| `Game/GameManager.cs` | Estados `Intro / Playing / Finished`, puntaje, contador de cristales, eventos para la UI. |
| `Game/GoalZone.cs` | El portal: **empieza bloqueado** y se abre al recoger el quinto cristal. |
| `Game/Collectible.cs` | Recogida de cristales. |
| `Game/TagUtil.cs` | Utilidad para buscar tags en la jerarquía. |
| `UI/StartScreen.cs`, `UI/HUDController.cs`, `UI/EndScreen.cs` | Las tres pantallas que pide la rúbrica. |
| `Audio/AudioDirector.cs` | Música ambiente + 5 efectos de evento. |
| `Editor/TallerFinalSetup.cs` | Menú **Taller Final** en la barra de Unity: reconstruye el prefab, crea el Animator Controller, corrige la cámara y hace un chequeo automático. |

Además: reorganización de `Assets/Scripts/` por responsabilidad, tags nuevos (`Jugador`, `Cristal`, `Meta`) y layers (`Jugador`, `Suelo`, `Muro`, `Entorno`), y el **bug del tag `Meta`** que estaba roto desde el commit `60b91cf` (la meta del avance 1 nunca se activaba).

**El diagrama de animaciones se genera automáticamente** desde el Animator Controller (`Docs/Diagrama_Animaciones.md`), así que el diagrama y la implementación no pueden discrepar — que es literal lo que pide la rúbrica.

---

## 2. Lo que necesito de vos — 3 entregables

### E1 — El humanoide (LO MÁS URGENTE)

1. Entrá a **mixamo.com** (cuenta Adobe gratis).
2. Descargá un personaje completo → **"X Bot"** o **"Y Bot"**.
3. Descargá las animaciones: **Idle**, **Walking**, **Running**, **Jump**.
4. Guardá todo en:
   - El modelo (`.fbx`) → **`Assets/Models/Player/`**
   - Los clips (`.fbx`, uno por animación) → **`Assets/Animations/`**

**Import settings del modelo** (en el Inspector del `.fbx`, pestaña *Rig*):
- `Animation Type` = **Humanoid**
- `Avatar Definition` = **Create From This Model**
- `Optimize Game Objects` = ✅
- `Scale` = **1.0** (Mixamo ya viene en escala correcta)
- Verificá que el **pivote esté en los pies** (pestaña *Animation* → *Configure → Pivot → Feet*)

**Nombres de los clips**: renombrá a `Idle`, `Walk`, `Run`, `Jump` (exactamente así, el script los busca por nombre).

> **Por qué es lo más urgente:** el humanoide + animaciones + diagrama son **20 % de la nota**. Sin el modelo, esa parte queda en cero y no hay forma de recuperarla en los 5 días que quedan. Todo lo demás ya está.

### E2 — El Terrain

- `Assets → Create → 3D Object → Terrain`, y movélo a **`Assets/Terrain/`**.
- `Terrain Settings`: `Heightmap Resolution` **1024**, `Size` **200 × 200 × 40**.
- Esculpí relieve con `Raise/Lower Terrain` (colinas de 3–8 m) y **aplaná** la zona donde va el laberinto.
- Material: usar **`t140.mat`** (*"Suelo Rocoso"*, que ya está en el repo) como Terrain Layer.
- Revisá que el **fog** está apagado (`m_Fog: 0` en las Environment Settings) y encendelo con color coherente con el cielo.

### E3 — Cristales y Portal

- **5 cristales** con nombres exactos: `Cristal_01` … `Cristal_05`. Tag `Cristal`, collider **Trigger**.
- **1 portal** con nombre exacto: `Portal`. Tag `Meta`, collider **Trigger**.
- El portal necesita **dos estados visuales**: `Portal_Sellado.mat` (apagado, sin luz) y `Portal_Activo.mat` (emisivo + una luz child). Mi código hace el swap en una línea; vos solo dejás los dos materiales.
- Distribuilos **dentro** del laberinto, en ramas distintas, para que el jugador tenga que explorar.

> **Todos los tags y layers ya están declarados** en `ProjectSettings/TagManager.asset`. No toques ese archivo.

---

## 3. El contrato (lo que mi código ya asume)

| Objeto | Regla |
|---|---|
| `Player` (raíz) | Tag `Jugador`. Lleva `CharacterController` (1.8 / 0.35 / center 0.9) + `PlayerController` + `GroundChecker`. **Escala 1,1,1** |
| `Model` (hijo de `Player`) | La malla del humanoide + `Animator` + `PlayerAnimationDriver` |
| `Cristal_01`…`Cristal_05` | Tag `Cristal`, collider Trigger |
| `Portal` | Tag `Meta`, collider Trigger, 2 materiales + luz child |
| `PuntoInicio` | Empty en el piso de entrada (lo creo yo si me decís dónde) |
| Parámetros del Animator | `Speed` (float), `Grounded` (bool), `Jump` (trigger) |

---

## 4. Cómo terminamos de unir todo

En Unity, menú **Taller Final**:

1. **1. Reconstruir prefab Player** → importa tu `.fbx` como hijo `Model` y deja el prefab listo.
2. **2. Crear Animator Controller y diagrama** → arma el `.controller` con tus clips y genera el diagrama.
3. **3. Corregir camara e instancia** → escala del jugador, tag y cámara en `(0, 2.5, −5)`.
4. **4. Chequeo rapido** → imprime en consola qué falta. ** Pegame la salida y seguimos.**

Ojo: el paso 1 **borra el `Rigidbody` y el `SphereCollider`** de la esfera del avance 1. Es intencional: un Rigidbody y un CharacterController en el mismo objeto se pelean por la posición.

---

## 5. Fechas

| Día | Fecha | Qué |
|---|---|---|
| **Hoy** | jue 26 | Código listo. **Tu entrega E1 es el bloqueo** |
| **1** | vie 27 | Prefab con humanoide + Animator + diagrama + UI funcionando — **GATE 1** |
| **2** | sáb 28 | Escena armada, portal, cristales, niebla, build jugable |
| **3** | dom 29 | **Feature freeze.** Solo bugs |
| **4** | lun 30 | Pruebas, PDF de entrega final, build, commit |
| **5** | mar 1 | **Entrega** |

**GATE 1 (viernes 27):** si el humanoide no está animado y la UI no muestra valores en vivo, se recorta alcance (props, transiciones, sonidos secundarios). **El humanoide, la animación, la UI y el puntaje no se tocan nunca.**

---

## 6. Lo que NO tenés que tocar

- ❌ `Assets/Scripts/**` — es mío.
- ❌ `Assets/Prefabs/Player/**` — lo genero yo con el menú.
- ❌ `Assets/Scenes/**` — la escena la armo yo.
- ❌ `ProjectSettings/TagManager.asset` — ya está con los tags y layers.
- ❌ Los assets del avance 1 (`Assets/Models/`, `Assets/Materials/`, `Assets/Textures/`) — solo agregá, no muevas ni borres.
- ❌ La rama `main` — todo va en `feature/taller-final`.

**Consultá el plan:** `Plan_Trabajo_Taller_Final.md` (diagnóstico, cronograma con gates, orden de recorte si nos atrasamos, y el protocolo de escala de la escena — §11 es importante para T-A2).

---

## 7. Una nota honesta sobre el plazo

Perdimos los días 24 y 25, así que vamos con **5 días en vez de 8**. Lo que hice hoy fue **todo** el código, justamente porque el código no depende de tus assets y eso era lo único que podía avanzar sin bloquearse.

Lo que falta es exactamente tu parte, y hay **20 % de la nota** (humanoide + animaciones + diagrama) que depende enteramente de que el `.fbx` de Mixamo esté en `Assets/Models/Player/`.

**Si tenés el humanoide hoy, el jueves 27 tenemos el GATE 1 en verde y llegamos.** Si no, decímelo y replanteamos el alcance juntos.

---

*Cualquier duda, preguntame antes de tocar algo — más vale 3 minutos de chat que media hora de trabajo perdido.*
