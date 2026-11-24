# Plan de trabajo — Taller Final (Laberinto 3D)

**Proyecto:** `Taller1Unity` · **Entregable:** `Taller_Final.md` · **Base:** avance 1 (commit `481714e`)

---

## 0. Cómo leer este documento

1. La **sección 1** es el diagnóstico: qué del avance 1 ya sirve y qué falta.
2. La **sección 2** son las **tres decisiones bloqueantes**. Hay que cerrarlas JUNTOS antes de escribir código. Son de 30 minutos y evitan semanas de retrabajo.
3. La **sección 3** es el **contrato de equipo**: los nombres, rutas y APIs que cada uno asume del otro. Sin esto, cada script se inventa sus propias referencias.
4. Las **secciones 4 y 5** son el plan de trabajo de cada uno, tarea por tarea, con el paso a paso.
5. La **sección 6** es la **secuencia**: qué bloquea a qué.
6. La **sección 7** es el checklist de integración y pruebas.
7. La **sección 8** es el checklist de entrega.

> **Convención de idioma (decidida):** código, nombres de clase/método/campo, nombres de prefab, parámetros de Animator y nombres de GameObjects nuevos → **inglés**. La documentación y los comentarios → **español**. **No renombrar los GameObjects existentes** (`Piedras`, `Laberinto`, `Piso`, `Contornos`, `Gargolas`): son referencias de escena y renombrarlos rompe la jerarquía sin ganar nada.

---

## 1. Diagnóstico: cobertura del avance 1

### 1.1 Matriz por sección del taller

| § | Requisito | Estado | Evidencia en el repo |
|---|---|---|---|
| **1** | Escenario construido | 🟢 | `MainScene.unity`: 105 GameObjects, 73 MeshRenderers, 26 MeshColliders, 23 instancias de prefab de modelo |
| 1 | **Terrain configurado y modificado** | 🔴 | 0 objetos Terrain. Todo el suelo es geometría importada plana |
| 1 | **Cambios de altura / relieve** | 🔴 | Escena 100 % plana, sin elevación |
| 1 | Materiales/texturas aplicados | 🔴 | Hay 7 materiales buenos (`t140`, `t181`, `t183`, `t184`, `ConcretePavers`, `MetalCorroded`, `skybox4`) pero **no hay terreno al que aplicarlos** |
| 1 | Objetos 3D distribuidos | 🟢 | 23 prefabs: arco romano, fuente rústica, rocas (Poliigon), gárgolas, musgo |
| 1 | Intención artística reconocible | 🟢 | Gárgola + arco romano + musgo + cielo propio → tema "ruinas incursadas" coherente |
| 1 | **Skybox no predeterminado** | 🟢 | `skybox4.mat` asignado en `RenderSettings`, cubemap de 6 caras (`Assets/Textures/skybox3/`) |
| 1 | Iluminación coherente | 🟡 | 1 Directional Light cálido (intensidad 1). Ambiente desde skybox (`m_AmbientMode: 0`). Sin luces de acento ni sombras urbanas |
| 1 | **Fog / niebla configurada** | 🔴 | `m_Fog: 0` → **apagada**. El modo y el color ya están seteados (mode 3, Exponential Squared): hay que encenderla y ajustar |
| **2** | **Personaje humanoide** | 🔴 | Es una **esfera**. Hay que conseguir/crear el modelo |
| 2 | Desplazarse | 🟢 | `AddForce` en `PlayerController.cs` |
| 2 | Responder a entradas | 🟢 | WASD + flechas vía `Keyboard.current` |
| 2 | Contacto con el terreno | 🟡 | Gravedad + Rigidbody, sin control explícito de suelo |
| 2 | Interactuar con colisiones | 🟢 | `OnCollisionEnter` con tag `Muro` |
| 2 | **Varias animaciones (Idle→Walk→Run→Jump)** | 🔴 | 0 AnimationClip, 0 Animator |
| **3** | **Diagrama de estados == Animator** | 🔴 | No existe ninguno de los dos |
| **4** | Adelante / atrás | 🟢 | `W/S` + flechas |
| 4 | Lateral (desplazamiento o rotación) | 🟢 | `A/D` |
| 4 | **Rotación de cámara/personaje** | 🔴 | La cámara *persigue* la posición del jugador; **no hay mouse look** |
| 4 | **Acción adicional** (saltar / correr / interactuar) | 🔴 | Ninguna |
| 4 | **El movimiento se detiene al soltar** | 🟡 | `m_LinearDamping: 0` + `AddForce` sin tope → la esfera no frena |
| **5** | Collider | 🟢 | 69 BoxCollider + 26 MeshCollider, ajustados a la geometría |
| 5 | Trigger | 🟡 | 4 triggers de meta correctamente configurados, **pero el tag `Meta` no existe** (bug heredado) |
| 5 | Rigidbody o Character Controller | 🟢 | Rigidbody en el Player |
| 5 | **Detección de suelo** | 🔴 | No hay raycast ni check de suelo. Sin salto no se puede hacer |
| **6** | Objetivo identificable y verificable | 🟡 | "Llegar a la meta" existe pero **no se ejecuta** (tag roto) |
| 6 | **"No debe limitarse a caminar libremente"** | 🟡 | Recorrido libre hasta un destino → **justo en el borde** del criterio |
| 6 | Condición de finalización | 🟡 | En código (`isKinematic = true` + log), sin ejecutar |
| **7** | **Indicador de progreso permanente** | 🔴 | No hay HUD. Solo `Debug.Log` con "Puntaje: 1000" hardcodeado |
| **7** | El puntaje refleja la lógica | 🟡 | El contador de impactos sí es real, pero no se muestra |
| **8** | **Pantalla inicial** (nombre, objetivo, controles) | 🔴 | 0 Canvas, 0 RectTransform, 0 TextMeshPro en todo el proyecto |
| 8 | HUD de progreso durante la ejecución | 🔴 | — |
| 8 | **Pantalla final** + bloquear puntuación posterior | 🔴 | — |
| **9** | **Sonido ambiental o música** | 🔴 | 0 AudioSource en la escena |
| 9 | **Dos sonidos de evento** | 🔴 | — |
| 9 | Volumen sin saturación | 🔴 | — |
| **Rúbrica** | Organización y calidad | 🟡 | Carpetas planas (`Assets/Scripts/` sin subcarpetas), `Taller_Final.md` y `Documento_Entrega.pdf` sin trackear, `CameraFollow.cs` contiene una clase llamada `ThirdPersonFollow`, 0 tests |

### 1.2 Puntaje estimado actual: **~40 / 100**

| Criterio | Peso | Est. actual | Est. con el plan |
|---|---|---|---|
| Escenario, Terrain, ambientación | 10 % | 5 % | 9 % |
| Estilo artístico y composición | 10 % | 6 % | 9 % |
| Modelos 3D y construcción | 10 % | 7 % | 9 % |
| Humanoide y animaciones | 10 % | **0 %** | 9 % |
| Diagrama y Animator Controller | 10 % | **0 %** | 9 % |
| Movimiento y entradas | 10 % | 5 % | 9 % |
| Colliders, triggers y física | 10 % | 7 % | 9 % |
| Objetivo, lógica y funcionalidad | 10 % | 3 % | 9 % |
| UI, puntaje e información | 8 % | **0 %** | 7 % |
| Sonido | 5 % | **0 %** | 4 % |
| Organización y calidad | 7 % | 4 % | 6 % |
| **Total** | **100 %** | **~37 %** | **~89 %** |

> Estimación propia, leída contra la rúbrica, no una nota del profesor. Sirve para priorizar, no como promesa.

### 1.3 Hallazgo transversal: la arquitectura de control hay que cambiarla

El avance 1 usa **Rigidbody + `AddForce` sobre una esfera**. Un humanoide con animaciones no funciona con eso: la animación sería cosmética sobre un cuerpo que resbala, y la rúbrica pide *"mantener contacto adecuado con el terreno"* y *"detección de suelo"*.

**Recomendación: migrar a `CharacterController`.** Es el estándar de Unity para third-person, la rúbrica lo admite explícitamente (*"Rigidbody o Character Controller"*), y habilita de una sola vez: salto con ground-check, frenado correcto, cámara orbital y Animator con parámetros limpios.

> ✅ **Decisión tomada (D3, 2026-11-24): la esfera se descarta y se reemplaza por el humanoide de Mixamo.** El `Player.prefab` se reconstruye con esta jerarquía:
>
> ```
> Player            [tag: Jugador]   ← CharacterController + PlayerController
>  │                                  + GroundChecker + PlayerAnimationDriver
>  └── Model        [tag: Untagged]  ← malla del humanoide + Animator
> ```
>
> El `Animator` va en el **hijo** `Model` (es lo que se anima); el `CharacterController` va en la **raíz** (es lo que mueve). Si se pone el Animator en la raíz, el humanoid se anima pero el controlador lo teletransporta; si se pone el CharacterController en el hijo, el personaje atraviesa el suelo.
>
> **El Rigidbody se elimina del jugador.** Poner un Rigidbody y un CharacterController en el mismo GameObject es el clásico conflicto: los dos se pelean por la posición y el personaje vibra o atraviesa los muros. Los Rigidbody quedan solo donde hagan falta (props con física, si los hay).
>
> **La detección de impactos con muros hay que migrarla.** El avance 1 usa `OnCollisionEnter`, que es un callback de Rigidbody. `CharacterController` **no** dispara `OnCollisionEnter`: dispara `OnControllerColliderHit` (*"called when the controller hits a collider while performing a Move"*). Sin este cambio, el contador de impactos y el `SFX_Impacto` dejan de funcionar en silencio.
>
> Los **triggers no se ven afectados**: `OnTriggerEnter` requiere que ambos objetos tengan collider y que al menos uno sea trigger y al menos uno sea *physics body collider* — el `CharacterController` cumple ese rol, así que los triggers de cristal y de portal siguen disparando normalmente.

---

## 2. Decisiones bloqueantes (cerrar JUNTOS, 30 min)

> Estas tres decisiones cambian qué código se escribe y qué assets se download. **No arrancar tareas hasta cerrarlas.**
>
> ✅ **Cerradas el 2026-11-24** (D1 → A, D2 → A, D3 → A + reemplazo de la esfera). Ver "Resultado de las decisiones" más abajo. Quedan abiertas D5, D6, D7 y D9.

### D1 — Mecánica de objetivo: ¿qué se recoge?

| Opción | Descripción | Valoración |
|---|---|---|
| **A. Cristales + portal** ⭐ | 5 cristales dispersos por el laberinto + llegar al portal de salida con los 5 | **Es el ejemplo textual del enunciado** ("Encuentra los cinco cristales ocultos y llega al portal de salida", "Cristales encontrados: 5 / 5"). Resuelve §6, §7 y §8 de un solo golpe con el mismo dato. |
| B. Solo llegar al portal | Mantener el avance 1 | El rubric penaliza *"limitarse a caminar libremente"*. Riesgo alto en §6. |
| C. Activar puntos de interés | N puntos que se encienden al acercarse | Más trabajo de escena y menos claro que A. |

**→ Recomendación: A.** Modo de juego final: *"es Exploración del Laberinto: reuní los 5 cristales ocultos y llegá al portal"*.

### D2 — ¿Terrain de Unity o terreno de geometría?

| Opción | Descripción | Valoración |
|---|---|---|
| **A. Unity Terrain** ⭐ | Un Terrain real, esculpido con hills/valleys, con material "Suelo Rocoso" (`t140`) | Cumple literal *"un terreno configurado y modificado"* + *"cambios de altura perceptibles"*. Además, Unity lo exige para el criterio 1. |
| B. Geometría de modelos + escalonado de pisos | Reinterpretar la rúbrica como "combinación justificada de terreno y geometría" | Se puede argumentar, pero "cambios de altura perceptibles" queda flojo y el profesor lo evalúa mal. |

**→ Recomendación: A**, y **conservar el laberinto de cubos encima** (ese laberinto es el activo más valioso que tienen). El Terrain es el suelo y el relieve; el laberinto queda como geometría construida sobre él.

### D3 — ¿De dónde sale el humanoide y sus animaciones?

| Opción | Descripción | Valoración |
|---|---|---|
| **A. Mixamo** ⭐ | Descargar de mixamo.com (Adobe, cuenta gratis): modelo + Idle/Walk/Run/Jump | Legal, gratis, calidad y variedad de clips estandarizados. Es el atajo real. |
| B. Mixamo + rig propio | Personaje propio, rig y clips a mano | Se va de las horas; sólo si el curso ya pasa por rigs |
| C. humanoid genérico (Unity Starter Assets / mannequin) | Ya viene con rig y animaciones de ejemplo | Rápido, pero se ve "Unity demo". Riesgo en *"estilo artístico y composición"* (10 %) |

**→ Recomendación: A** (Mixamo). **Andres** lo descarga y lo deja importado con las opciones correctas (ver T-A3).

> ✅ **Decisión tomada (2026-11-24): opción A — Mixamo.** Modelo + clips `Idle`, `Walk`, `Run`, `Jump`.
> **Además: la esfera actual se reemplaza por el humanoide** (ver §1.3). El `Player.prefab` se reconstruye; la esfera no se conserva en el juego.

### Resultado de las decisiones

| Decisión | Elegido | Fecha | Consecuencia principal |
|---|---|---|---|
| **D1** Mecánica de objetivo | **A — 5 cristales + portal** | 2026-11-24 | El objetivo obliga a explorar (§6). Un solo contador alimenta el objetivo, el HUD y el puntaje (§7, §8). T-A5 y T-S6 |
| **D2** Terreno | **A — Unity Terrain** | 2026-11-24 | Cumple "terreno modificado" + "relieve perceptible". **Implica reescalar el laberinto** (T-A2) antes del tuning de movimiento (T-S3 Fase 2) |
| **D3** Origen del humanoide | **A — Mixamo**, y la esfera se reemplaza | 2026-11-24 | El `Player.prefab` se reconstruye con `CharacterController` + hijo `Model` con `Animator`. El `Rigidbody` del jugador desaparece (§1.3) |
| **D4** ¿Se conserva el laberinto de cubos? | **Sí** (implícito en D2-A) | 2026-11-24 | Se conserva como geometría construida sobre el Terrain. Es el activo más valioso del avance 1 |
| **D8** ¿Se usa la esfera en algún momento? | **No** (implícito en D3) | 2026-11-24 | La esfera se elimina del proyecto. `Player.prefab` viejo queda como referencia hasta que T-S3 lo reemplace |
| **D5** Regla de puntaje | **A — `+100`/cristal, `+1`/seg, `−10`/impacto, clamp a 0** | 2026-11-24 | La colisión (§5) queda visible en el puntaje (§7). Bloquea el HUD y el `GameManager` |
| **D6** ¿Enemigo o hazard? | **No — ninguno** | 2026-11-24 | Evita +1 IA, +1 animación y una condición de derrota ambigua. Los muros del laberinto ya cubren "superar obstáculos" |
| **D7** Nombre de la experiencia | **"El Laberinto de la Gárgola"** | 2026-11-24 | Ata al asset más distintivo de la escena → juega a favor del 10 % de coherencia artística |
| **D1b** ¿El portal exige los 5 cristales? | **A — bloqueado hasta juntarlos** | 2026-11-24 | El portal se enciende al recoger el último → señal visible desde lejos. **Andres debe modelar dos estados visuales** (T-A5) |

### Decisiones que siguen abiertas

| # | Decisión | Por qué sigue abierta | Impacto si se demora |
|---|---|---|---|
| **D9** | Fecha de entrega | Es la que define el ritmo | No bloquea ninguna tarea técnica. Si se aprieta, define **qué se recorta**: primero sonido, después polish de ambiente; humanoide/animaciones y UI son irrenunciables |

---

## 3. Contrato de equipo

> La mitad de los conflictos de un trabajo en dos surge de que cada uno **[asumió]** una referencia que el otro no creó. Esta sección es la lista explícita de esas suposiciones.

### 3.1 Quién toca qué (superficies de edición)

| Zona | Sebastian (código) | Andres (modelos / ambientación) |
|---|---|---|
| `Assets/Scripts/**` | ✅ exclusivo | ❌ |
| `Assets/Scenes/**` — scripts, UI, Animator, prefabs de gameplay | ✅ exclusivo | ❌ |
| `Assets/Animations/**` | solo el `.controller` | ✅ clips + modelo |
| `Assets/Models/**`, `Assets/Materials/**`, `Assets/Textures/**` | ❌ | ✅ exclusivo |
| `Assets/Terrain/**` | ❌ | ✅ exclusivo |
| `Assets/Audio/**` | ✅ *scripting* de reproducción | ✅ *clips* |
| `Assets/Prefabs/Player/**`, `Assets/Prefabs/UI/**` | ✅ | ❌ |
| `Assets/Prefabs/Collectibles/**`, `Assets/Prefabs/Environment/**` | ❌ | ✅ |

**Regla dura:** si un cambio del otro te rompe, no lo parchees en tu zona. Avisá.

### 3.2 Estructura de carpetas objetivo

```
Assets/
├── Scenes/
│   ├── MainScene.unity
│   └── MainScene_UI.unity          (opcional: UI en escena aparte)
├── Scripts/
│   ├── Player/      PlayerController.cs, PlayerAnimationDriver.cs, GroundChecker.cs
│   ├── Camera/      CameraOrbit.cs
│   ├── Game/        GameManager.cs, Collectible.cs, GoalZone.cs
│   ├── UI/          StartScreen.cs, HUDController.cs, EndScreen.cs
│   └── Audio/       AudioDirector.cs
├── Prefabs/
│   ├── Player/      Player.prefab
│   ├── Collectibles/Collectible.prefab
│   ├── Environment/Portal.prefab
│   └── UI/           GameHUD.prefab
├── Models/          (ya existe)
├── Materials/       (ya existe)
├── Textures/        (ya existe)
├── Terrain/         ← NUEVO
├── Animations/      ← NUEVO  (clips)
└── Audio/           ← NUEVO
    ├── Music/
    └── SFX/
```

### 3.3 Contrato de nomenclatura y referencias

**Tags requeridos** (en `ProjectSettings/TagManager.asset`):

| Tag | Quién lo crea | Para qué |
|---|---|---|
| `Jugador` | Sebastian | identificar al humanoide |
| `Cristal` | Sebastian | coleccionables |
| `Meta` | Sebastian | portal de salida — **además de arreglar el bug heredado** (ver T-S10) |
| `Muro` | ya existe | no tocar |

**Layers requeridos:**

| Layer | Quién | Para qué |
|---|---|---|
| `Jugador` | Sebastian | que el raycast de suelo no se golpee a sí mismo |
| `Suelo` | Sebastian | ground-check: sólo `Suelo` y `Default` cuentan como piso |
| `Muro` | Sebastian | distinguir paredes (opcional, para sonido de impacto) |
| `Entorno` | Sebastian | props decorativos |

**Nombres de GameObjects que el código de Sebastian va a buscar** — si Andres no los respeta, el código no los encuentra:

| GameObject | Quién lo crea | Requisito |
|---|---|---|
| `Player` (raíz del humanoide) | Sebastian (prefab) / Andres (modelo) | Tag `Jugador`. Lleva `CharacterController` + `PlayerController` + `GroundChecker` + `PlayerAnimationDriver` |
| `Model` (hijo de `Player`) | Sebastian (prefab) / Andres (modelo) | Malla del humanoide + `Animator`. Es lo que se anima; la raíz es lo que se mueve |
| `Camara` | Sebastian | La cámara principal, con el script de órbita |
| `Cristal_01` … `Cristal_05` | Andres | Tag `Cristal`, collider tipo **Trigger** |
| `Portal` | Andres | Tag `Meta`, collider tipo **Trigger**, con los **dos materiales** (`Portal_Sellado` / `Portal_Activo`) y una luz child apagada por defecto |
| `PuntoInicio` | Sebastian | Empty con posición de spawn del jugador |
| `Terrain` | Andres | Nombre exacto (Unity lo exige) |

> **Ojo:** el objeto `Meta` que ya existe en la escena es un **padre vacío** que agrupa los 4 triggers del avance 1. Con la mecánica D1-A ese grupo se reemplaza por el `Portal` único. Decidir quién lo borra (recomendación: Sebastian, cuando integre el portal).

**Contrato de Animator** (lo implementa Sebastian, los clips los entrega Andres):

| Parámetro | Tipo | Uso |
|---|---|---|
| `Speed` | float | 0 = idle, > 0.1 = walk, > 3.5 = run (blend tree) |
| `Grounded` | bool | Airborne ↔ Grounded |
| `Jump` | trigger | dispara la transición a Jump |
| `Interact` | trigger | transición a la animación de interacción |

**Contrato de clips (nombres que Sebastian espera en el Animator):**

| Clip | Estado | Origen |
|---|---|---|
| `Idle` | estado Idle | Mixamo |
| `Walk` | sub-state de Locomotion | Mixamo |
| `Run` | sub-state de Locomotion | Mixamo |
| `Jump` | estado Jump (Airborne) | Mixamo |
| `Interact` *(opcional)* | estado Interact | Mixamo |

> **Andres entrega los clips con esos nombres exactos** (o avisa la diferencia y Sebastian ajusta el Animator). Con `Import Settings → Loop Time` activo sólo en `Idle` / `Walk` / `Run`.

**Contrato de sonido (contrapartida de los 2 eventos del rubric):**

| Clip | Evento | Quién lo dispara |
|---|---|---|
| `Ambient_Bosque` / `Ambient_Ruinas` | loop de fondo | Sebastian (`AudioDirector`) |
| `SFX_Recoger` | al recoger un cristal | Sebastian |
| `SFX_LlegarMeta` | al completar | Sebastian |
| `SFX_Salto` *(opcional)* | al saltar | Sebastian |
| `SFX_Impacto` *(opcional)* | choque con muro | Sebastian |

**Contrato de UI (textos que Sebastian renderiza, arte que aporta Andres):**

- Sebastian define: `StartScreen` (título, objetivo, controles, "Presione una tecla"), `HUD` (puntaje, `Cristales: X / 5`, tiempo), `EndScreen` (completado, puntaje final, tiempo).
- Andres aporta: **paleta de color y tipografía** de la escena, para que la UI no desentone (criterio "estilo artístico" incluye la interfaz, 10 %).

### 3.4 Restricción del proyecto que hay que respetar

`ProjectSettings/ProjectSettings.asset` → `activeInputHandler: 1` = **solo Input System Package (New)**.

> ⛔ `Input.GetAxis`, `Input.GetKey` y `Input.mousePosition` **no funcionan** en este proyecto. Todo tiene que ir por `Keyboard.current`, `Mouse.current` o un `InputAction`. Si alguien escribe código con la API vieja, no compila el input (falla en runtime, no en el editor) y se pierde tiempo depurando.

---

## 4. Plan de trabajo — Sebastian (código)

> Orden interno sugerido: **T-S1 → T-S2 → T-S3 → T-S4 → T-S5 → T-S6 → T-S7 → T-S8 → T-S9 → T-S10**.
> T-S3, T-S4 y T-S7 **no se pueden terminar** hasta que Andes entregue terreno (T-A1/T-A2) y modelo (T-A3). Para no bloquearse, esas tareas tienen una **Fase 1 con assets provisorios**.

### T-S1 — Cerrar decisiones y fijar el contrato de la API 🟢 Bloqueante

**Objetivo:** que ninguna tarea posterior dependa de una suposición.

1. Reunirse 30 min con Andres y cerrar D1, D2, D3 (§2).
2. Completar la tabla "Resultado de las decisiones".
3. Escribir en este documento (sección 3.3) los valores finales: cantidad de cristales, tags, layers, nombres.
4. Crear la rama de trabajo: `git switch -c feature/taller-final` (no trabajar sobre `main`).
5. Commitear el material que hoy está suelto:
   - `git add Taller_Final.md Documento_Entrega.pdf`
   - `git commit -m "docs: add final workshop brief and delivery document"`

**Entregable:** este documento con D1/D2/D3 resueltas y la rama creada.

---

### T-S2 — Saneamiento del proyecto 🟢 Desbloquea a ambos

**Objetivo:** que el proyecto esté ordenado antes de que empiece a crecer.

1. Crear la estructura de carpetas de §3.2 (vacías con `.meta`generated por Unity).
2. Mover `Assets/Scripts/PlayerController.cs` → `Assets/Scripts/Player/PlayerController.cs`.
3. Mover `Assets/Scripts/CameraFollow.cs` → `Assets/Scripts/Camera/CameraOrbit.cs`, **renombrando la clase `ThirdPersonFollow` → `CameraOrbit`** (unificar nombre de archivo y de clase; hoy no coinciden).
4. Crear en `ProjectSettings/TagManager.asset` los tags `Jugador`, `Cristal`, `Meta` y las layers `Jugador`, `Suelo`, `Muro`, `Entorno`.
5. Instalar el paquete de **TextMeshPro** si no está: `Window → Package Manager → Unity Registry → "TextMeshPro" → Install`. (En Unity 6 suele venir incluido en `com.unity.ugui`; verificar que `TMP Essential Resources` esté importado: `Window → TextMeshPro → Import TMP Essential Resources`).
6. Verificar que `Assets/TutorialInfo/` (restos del template URP: `Readme.cs`, `URP.png`, `Layout.wlt`) no estorbe → se puede borrar, pero **solo con el visto bueno de Andres**, porque es el andamiaje de la presentación de Unity.

**Entregable:** proyecto con carpetas ordenadas, tags/layers declarados y TMP operativo.

---

### T-S3 — `PlayerController` v2: humanoide con `CharacterController` 🔴 Alto

**Objetivo:** reemplazar la esfera por el control de un humanoide con salto, carrera y frenado correctos.

**Fase 0 — reconstruir el prefab (antes de todo lo demás):**
1. Crear el prefab `Assets/Prefabs/Player/Player.prefab` con esta jerarquía exacta (§1.3):
   ```
   Player   [tag: Jugador]
    │        CharacterController (height 1.8, radius 0.35, center y 0.9, slopeLimit 50, stepOffset 0.3)
    │        PlayerController
    │        GroundChecker
    │        PlayerAnimationDriver
    └── Model
             malla del humanoide (hijo de Andres)
             Animator  ← va AQUÍ, no en la raíz
   ```
2. **Quitar el `Rigidbody` y el `SphereCollider` de la esfera.** No se arrastran: Rigidbody + CharacterController en el mismo objeto se pelean.
3. Verificar que el `Model` es hijo de la raíz y que su pivote está **en los pies** (si Mixamo lo deja en el centro, corregirlo en el prefab con un offset, no en el `.fbx`).

**Fase 1 — con assets provisorios** (para no esperar a Andres):
4. Implementar el movimiento con **velocity** en vez de `AddForce`:
   ```csharp
   void Update() {
       Vector3 dir = DireccionEntrada();                 // relativo a cámara
       Vector3 vel = characterController.velocity;
       vel.x = dir.x * velocidadActual;
       vel.z = dir.z * velocidadActual;
       vel.y = vel.y;                                     // conservar gravedad
       characterController.Move(vel * Time.deltaTime);
   }
   ```
   > Con `CharacterController` no se aplica `AddForce`: se asigna `velocity` y se llama `Move()`. Es la diferencia clave respecto al avance 1.
5. Implementar **correr**: `Shift` held → `velocidadActual = velocidadRun` (6–7 m/s) en vez de `velocidadWalk` (2.5–3 m/s).
6. Implementar **saltar**: `Espacio` → si `GroundChecker.EstaEnSuelo()` → `velocidad.y = velocidadSalto` (5 m/s) y disparar `Jump` al Animator.
7. Implementar **frenado**: al soltar las teclas, `velocity` en X/Z → 0 directo. **Nunca** con `linearDamping` (el avance 1 usa Rigidbody; este controlador no usa Rigidbody).
8. Dejar el input en el **Input System**: `Keyboard.current.wKey.isPressed`, etc. — **nada de `Input.GetKey`**.

**Migración obligatoria — la detección de impactos:**
9. El avance 1 cuenta impactos con `OnCollisionEnter`, que es un callback de **Rigidbody**. `CharacterController` **no** lo dispara. Reescribirlo como:
   ```csharp
   void OnControllerColliderHit(ControllerColliderHit hit) {
       if (hit.collider.CompareTag("Muro")) {
           cantidadImpactos++;
           Debug.Log("Impacto registrado. Total: " + cantidadImpactos);
       }
   }
   ```
   > Sin este paso el contador de impactos y el `SFX_Impacto` quedan **mudos**: no tiran error, simplemente nunca se disparan.
10. Borrar del script viejo el `OnCollisionEnter`, el `OnTriggerEnter` de meta y el `rb.isKinematic = true` (esa lógica pasa al `GameManager` + `GoalZone`, ver T-S6).

**Fase 2 — con el modelo real de Andres:**
11. Meter el humanoide de Andres como hijo `Model` del prefab.
12. Ajustar `altura`/`radio` del `CharacterController` a la escala real del modelo (si Mixamo mide 1.8 m de alto y viene en escala 1.0, esos valores funcionan directo).
13. Si el `Model` quedó más chico o más grande que 1.8 m, ajustar **el CharacterController, no la escala del modelo** — deformar el rig rompe la animación.

**Entregable:** `Player.prefab` con humanoide, `CharacterController`, salto, carrera, frenado e impactos migrados a `OnControllerColliderHit`.

---

### T-S4 — `GroundChecker` y `CameraOrbit` con mouse look 🔴 Alto

**Objetivo:** detección de suelo fiable + cámara orbital con mouse.

1. Crear `Assets/Scripts/Player/GroundChecker.cs`:
   - `Raycast` hacia abajo desde el centro del `CharacterController`, longitud = `groundCheckDistance` (0.6) + radio.
   - Filtrar por layer `Suelo`/`Default` usando `~LayerMask` (excluir la layer `Jugador`).
   - Exponer `public bool EstaEnSuelo { get; private set; }` y `public Vector3 NormalSuelo`.
   - Usar `SphereCast` en vez de `Raycast` para no perder el contacto en escalones.
2. Crear `Assets/Scripts/Camera/CameraOrbit.cs`:
   - `Mouse.current` → deltas X/Y (usar `Mouse.current.delta.ReadValue()`).
   - `yaw` y `pitch` acumulados, con `pitch` clampeado a [-60°, +25°].
   - Rotar el **holder** de la cámara en Y y la cámara en X (o usar `transform.Rotate`).
   - **Cursor lock**: `Cursor.lockState = CursorLockMode.Locked` + `Cursor.visible = false` al iniciar el juego; liberar con `Esc` (en Unity 6, `Input System → UI Input Module` no lo fuerza; se setea a mano).
   - Mantener el **offset** (altura/distancia) y el suavizado, pero cambiar el Lerp a **decaimiento exponencial**:
     ```csharp
     float t = 1f - Mathf.Exp(-suavizado * Time.deltaTime);
     transform.position = Vector3.Lerp(transform.position, posicionDeseada, t);
     ```
     El Lerp con `suavizado * deltaTime` del avance 1 es dependiente del framerate.
3. **Reconciliar con el movimiento relativo a cámara:** como ahora la cámara rota con el mouse, `PlayerController` sigue tomando `Camera.main.transform.forward/right` aplanados. Verificar que `up` no contamine (el avance 1 ya lo aplana: `camForward.y = 0` → mantenerlo).
4. **Eliminar el feedback loop** del avance 1 (la cámara seguía la *dirección de la velocidad* y eso rotaba la cámara al stafejar). Con cámara orbital por mouse, la cámara ya no depende de la velocidad → el loop desaparece por diseño. Borrar `ultimaDireccion` y el bloque de `linearVelocity` del script viejo.

**Entregable:** salto condicionado a suelo real + cámara que responde al mouse sin arrastrar el movimiento.

---

### T-S5 — Animator Controller + diagrama 🟡 Medio

**Objetivo:** que el humanoide se anime y que el diagrama coincida **exactamente** con lo implementado (10 % de la nota, y es fácil de perder si no se hace en tandem).

1. Recibir los clips de Andres (T-A4) e importarlos con:
   - `Rig` = **Humanoid**, `Avatar` = **Create From This Model** (ya lo hizo en T-A3).
   - `Animation Type` = Humanoid.
   - `Loop Time` ✅ sólo en `Idle`, `Walk`, `Run`. ❌ en `Jump`.
2. Crear `Assets/Animations/Player.controller`:
   - **Parámetros:** `Speed` (float), `Grounded` (bool), `Jump` (trigger), `Interact` (trigger, opcional).
   - **Estados:** `Idle`, `Locomotion` (sub-state machine), `Jump`.
   - **Locomotion:** `Blend Tree` en Y sobre `Speed` con umbrales `0` (Idle), `0.1` (Walk), `3.5` (Run).
   - **Transiciones:**
     | Desde | Hacia | Condición |
     |---|---|---|
     | Idle | Locomotion | `Speed > 0.1` |
     | Locomotion | Idle | `Speed < 0.1` |
     | Locomotion | Jump | `Jump` (trigger) |
     | Jump | Locomotion | `Grounded == true` |
   - **Exit Time** en las transiciones a `Idle` para que no se corte la animación.
3. Crear `Assets/Scripts/Player/PlayerAnimationDriver.cs`:
   - Actualizar `Speed` = `characterController.velocity.magnitude` (o la velocidad horizontal).
   - Actualizar `Grounded` desde `GroundChecker.EstaEnSuelo`.
   - Detectar el salto → `SetTrigger("Jump")`.
   - Opcional: `Animator.SetFloat` con damping para que la blend no sea nerviosa.
4. **Dibujar el diagrama** que documente ese Controller:
   - Formato: **Mermaid** en `Docs/Diagrama_Animaciones.md` (versionable, texto, revisable en el repo) **+** una imagen PNG exportada para el PDF de entrega.
   - El diagrama debe mostrar: estados, parámetros, condiciones y transiciones **idénticas** a las del paso 2.
   - Ejemplo Mermaid:
     ```mermaid
     stateDiagram-v2
         [*] --> Idle
         Idle --> Locomotion : Speed > 0.1
         Locomotion --> Idle : Speed < 0.1
         Locomotion --> Jump : Jump [trigger]
         Jump --> Locomotion : Grounded == true
     ```
5. **Verificar correspondencia con Andres:** que él mire el diagrama y el Controller y confirme que coinciden.

**Entregable:** `Player.controller` funcionando + `Diagrama_Animaciones.md` que lo describe exactamente.

---

### T-S6 — `GameManager` + objetivo + puntaje 🟡 Medio

**Objetivo:** una fuente de verdad del estado del juego, y que §6 deje de ser "caminar hasta un destino".

1. Crear `Assets/Scripts/Game/GameManager.cs` con una máquina de estados simple:
   ```csharp
   public enum GameState { Intro, Playing, Finished }
   public GameState Estado { get; private set; }
   public int CristalesRecogidos { get; private set; }
   public int Puntaje { get; private set; }
   public float TiempoTranscurrido { get; private set; }
   ```
   - `Awake()`: `Estado = Intro` (bloquea el movimiento hasta que se pulse una tecla).
   - `Update()`: si `Estado == Playing` → acumula tiempo.
   - Eventos (C# `event Action`) para que la UI se suscriba: `OnCristalRecogido`, `OnPuntajeCambiado`, `OnJuegoFinalizado`.
   - **Puntaje real** (decisión D5, cerrada), no el "1000" hardcodeado del avance 1:
     | Regla | Valor | Nota |
     |---|---|---|
     | Cristal recogido | `+100` | 5 cristales = 500 base |
     | Segundo sin impactos | `+1` | Se acumula en `Update()` mientras `Estado == Playing` |
     | Impacto con muro | `−10` | Alimentado por `OnControllerColliderHit` (T-S3, paso 9) |
     | **Clamp** | `Puntaje = Mathf.Max(0, Puntaje)` | Imprescindible: sin esto el puntaje puede dar negativo y se ve roto en la pantalla final |
   - Rango esperado: ~600–900 en una partida normal, que calza con el ejemplo del enunciado (*"Puntaje final: 850"*).
   - Esta regla **conecta dos criterios**: la detección de colisiones (§5) queda visible en el puntaje (§7). El enunciado lo exige literal: *"El puntaje no puede ser únicamente decorativo; debe cambiar como consecuencia de la lógica implementada"*.
2. Crear `Assets/Scripts/Game/Collectible.cs`:
   - `OnTriggerEnter` → si `other.CompareTag("Jugador")` → avisar al `GameManager`, reproducir `SFX_Recoger`, desactivar el objeto.
   - Al recoger el **quinto** cristal → avisar al `GoalZone` para que active el portal (ver punto 3).
3. Crear `Assets/Scripts/Game/GoalZone.cs` — **el portal está BLOQUEADO hasta juntar los 5** (decisión D1b):
   - `Activo` (bool, arranca en `false`).
   - `OnTriggerEnter` → si `other.CompareTag("Jugador")`:
     - si `Activo` → **finalizar** (llamar a `GameManager.Finalizar()`).
     - si `!Activo` → feedback: *"El portal está sellado — faltan N cristales"*. **No** finalizar.
   - Método `Activar()` → se llama cuando se recoge el quinto cristal: enciende el portal visualmente (luz + material emisivo, ver T-A5) y pone `Activo = true`.
   - **El trigger sigue siempre activo**: el jugador tiene que poder acercarse al portal para leer el mensaje de "faltan N". El bloqueo es lógico y visual, no físico.
4. **Remover** la lógica vieja de meta del `PlayerController` (`isKinematic = true` + log). Ya no aplica: el rigido ahora es `CharacterController`.
5. **Congelar** el juego al finalizar: `Estado = Finished` → el `PlayerController` deja de consumir input (y opcionalmente `Time.timeScale = 0`).

**Entregable:** estado de juego centralizado, puntaje real con la regla D5, portal bloqueado que se activa con el quinto cristal.

---

### T-S7 — UI completa (pantalla inicial, HUD, pantalla final) 🔴 Alto

**Objetivo:** 8 % de la nota y el ítem que más se nota en una demo.

1. Crear el prefab `Assets/Prefabs/UI/GameHUD.prefab` (con el `Canvas` y las tres pantallas como hijos) y una escena `Assets/Scenes/MainScene_UI.unity` si se prefiere separar la UI del gameplay.
   - `Canvas` → `Render Mode: Screen Space - Overlay`, `CanvasScaler` → `Scale With Screen Size`, `Reference Resolution: 1920×1080`.
2. **Pantalla inicial** (`StartScreen.cs`):
   - Título: **"EL LABERINTO DE LA GÁRGOLA"** (decisión D7). Mayúsculas y con tracking amplio para que se lea como título de juego, no como un label.
   - Objetivo: *"Encontrá los cinco cristales ocultos y llevá el portal de salida."*
   - Controles: WASD, Mouse, Shift, Espacio.
   - *"Presione una tecla para comenzar."*
   - Al presionar una tecla → `GameManager.Estado = Playing` + capturar el mouse.
3. **HUD** (`HUDController.cs`), **siempre visible durante la ejecución**:
   - `Puntaje: 350`
   - `Cristales: 4 / 5`
   - `Tiempo: 02:43`
   - Suscrito a los eventos del `GameManager` → **no** hacer polling con `Update()`.
   - Opcional pero recomendado: `Impactos: 7` en un tamaño más chico. Da visibilidad al `−10` por impacto de la regla D5, para que el jugador entienda por qué baja el puntaje.
4. **Pantalla final** (`EndScreen.cs`):
   - `"MISIÓN COMPLETADA"`
   - `Cristales encontrados: 5 / 5`
   - `Puntaje final: 850`
   - `Tiempo: 02:43`
   - Liberar el mouse.
5. **Regla del rubric** (*"el usuario no debería continuar acumulando puntos"*): el `GameManager` bloquea el estado `Playing` al finalizar → el HUD queda congelado y la pantalla final no se puede saltarse.
6. Texto en español, **paleta coherente con la escena** (la que pase Andres, §3.3).

**Entregable:** las tres pantallas, con los valores en vivo desde el `GameManager`.

---

### T-S8 — Sonido 🟡 Medio

**Objetivo:** 5 % de la nota, y es lo más rápido de cerrar.

1. Crear `Assets/Audio/Music/` y `Assets/Audio/SFX/`. Andres deja los clips ahí (§3.3).
2. Crear `Assets/Audio/AudioMixer.ampl` con 3 grupos: `Master`, `Music`, `SFX`.
   - Volúmenes: Master 0 dB, Music **-12 dB**, SFX **-6 dB**.
   - El objetivo es que la música se oiga pero **no tape** los efectos (el rubric lo pide literal: *"sin generar saturación"*).
3. Crear `Assets/Scripts/Audio/AudioDirector.cs`:
   - `AudioSource` de música con `Loop = true`, `Play On Awake`.
   - `AudioSource` de efectos (2–3 instancias con `PlayOneShot`).
   - Métodos: `ReproducirMusica()`, `TocarRecoger()`, `TocarMeta()`, `TocarSalto()`, `TocarImpacto()`.
4. Conectar con el `GameManager`: `SFX_Recoger` al recoger, `SFX_LlegarMeta` al completar.
5. Verificar que los eventos suenan **una sola vez** (no en loop) y que el ambiente es el único loop.

**Entregable:** mixer con 3 grupos + `AudioDirector` conectado a los eventos.

---

### T-S9 — Sonido e iluminación: los "toques" que el rubric no ve pero se sienten 🟢 Bajo

1. **Andres**: activar la niebla y ajustar la iluminación → ver T-A6.
2. **Sebastian**: `SFX_Impacto` al chocar con tag `Muro` (opcional pero suma: usa el `OnCollisionEnter` que ya existe).

---

### T-S10 — Bugs heredados del avance 1 🔴 Alto

**Objetivo:** arreglar lo que ya está roto antes de construir encima.

> Este bug es el más urgente del proyecto: **sin él, la mecánica de meta no se ejecuta**. Está en la lista de "los bugs los dejamos para después", pero **al menos este hay que resolverlo ahora**, porque §6 del taller final exige una condición de finalización que funcione.

1. **Tag `Meta` no declarado** — el tag fue borrado del `TagManager.asset` en el commit `60b91cf` y nunca se restauró, pero 4 objetos de la escena siguen con `m_TagString: Meta`.
   - Con el Input System y la lógica nueva, el tag `Meta` vuelve a ser necesario para el `Portal` (T-S6).
   - Solución: agregarlo en el `TagManager.asset` (ya está en el alcance de T-S2, paso 4).
   - **Verificarlo:** abrir la escena, entrar a un trigger de meta y confirmar que no tira excepción.
2. **`m_LinearDamping: 0`** en el Rigidbody viejo → desaparece con la migración a `CharacterController` (T-S3). Si queda algún Rigidbody, poner damping.
3. **Lerp dependiente del framerate** en el suavizado de cámara → corregido en T-S4.
4. **Sin freno** → corregido en T-S3.
5. **Nombre de archivo ≠ nombre de clase** (`CameraFollow.cs` / `ThirdPersonFollow`) → corregido en T-S2.
6. **Limpieza de repo:** commitear `Taller_Final.md` y `Documento_Entrega.pdf` (T-S1), borrar restos del template (`Assets/TutorialInfo/`) con el visto bueno de Andres, y .gitignorear `Assembly-CSharp*.csproj` y `Taller1Unity.slnx` si no lo están ya.

**Entregable:** los 6 puntos resueltos y verificados en el Editor.

---

## 5. Plan de trabajo — Andres (modelos / ambientación)

> Orden: **T-A1 → T-A2 → T-A3 → T-A4 → T-A5 → T-A6 → T-A7**.
> **T-A1 y T-A2 son la ruta crítica del proyecto**: de ellos depende todo el tuning de movimiento de Sebastian.

### T-A1 — Crear y esculpir el Terrain 🔴 Crítico

**Objetivo:** el terreno que hoy no existe (criterio 1, 10 %).

1. `Assets → Create → 3D Object → Terrain`.
2. Moverlo a `Assets/Terrain/` (arrastrarlo) y renombrarlo **`Terrain`** (nombre exacto que pide Unity).
3. `Terrain → Terrain Settings`:
   - `Heightmap Resolution` = **1024** (buena calidad sinCastigar el peso del repo).
   - `Detail Resolution` = 512.
   - `Size` = **200 × 200 × 40** (metros). Ajustar a la escala del laberinto.
4. **Esculpir** con las herramientas de Paint:
   - `Raise/Lower Terrain` → colinas y una vaguada en el centro.
   - `Smooth Heightmap` → suavizar.
   - `Flatten` → aplanar la meseta donde va el laberinto (para que las paredes no queden flotando).
   - Objetivo: **cambios de altura perceptibles** de 3–8 m en el entorno, y **plano dentro** del laberinto.
5. `Paint Texture` → pintar la textura de grass/rock/soil (o dejar la de por defecto hasta que haya materiales).
6. Asignar material: usar `t140.mat` (*"Suelo Rocoso"*, de arqdecoskp, ya está en el repo) como **terrain layer**:
   - `Terrain → Materials → Edit Terrain Layers → Add Texture Layer` → asignar `t140`.
   - `Tile Size` ajustado al tamaño del terrain.
7. El Terrain trae su `TerrainCollider` automático → **verificar que el humanoide no se hunda ni floté** (depende de T-A2).
8. `Terrain → Settings → Draw Instanced` si el peso del repo molesta (opcional).

**Entregable:** Terrain con relieve esculpido, textura aplicada y collider funcionando.

---

### T-A2 — Reubicar y reescalar el laberinto sobre el Terrain 🔴 Crítico

**Objetivo:** unificar la escala. **Este es el punto donde se define el tuning de movimiento de Sebastian.**

1. Crear un empty `PuntoInicio` (hijo de `Laberinto`) y **dibujar el recorrido** con Gizmos o con un marker en el piso, para que Sebastian sepa dónde va el jugador y dónde el portal.
2. Mover el laberinto completo (`Laberinto`, `Contornos`, `Componentes`, `Piso*`, `Piedras`, `Gargolas`) para que quede **sobre** el Terrain y no atravesado.
3. **Escala unificada**: la escena actual mezcla escalas (el jugador mide ~1.25 de radio con `m_LocalScale 2.5`, y la cámara está a 45 unidades). Definir **1 unidad Unity = 1 metro** y dejar:
   - humanoid: escala **1.0**, altura **1.8 m**
   - muros del laberinto: altura **2.5–3 m**
   - `CharacterController`: altura 1.8, radio 0.35
4. Reubicar los 23 prefabs de modelo (arco romano, fuente, gárgolas, rocas) con la escala y rotación coherentes con la nueva escala.
5. **Revisar el cielo**: si el skybox es de 6 caras, la escala del cubemap define la "altura" del cielo → con una escena reescalada puede desalinearse. Ajustar el material del skybox si hace falta.
6. Entregar a Sebastian: **la posición exacta de `PuntoInicio`** y el **recorrido** (2–3 screenshots del editor con gizmos).

**Entregable:** escena coherente en escala + punto de spawn definido.

---

### T-A3 — Personaje humanoide (descarga + import correcto) 🔴 Crítico

**Objetivo:** el modelo que hoy no existe. **Este modelo reemplaza la esfera actual** (decisión D3) — no es un extra, es el reemplazo del jugador.

1. Descargar de **mixamo.com** (gratis con cuenta Adobe):
   - Modelo: **"X Bot"** o **"Y Bot"** (personaje completo; Mixamo ofrece variantes con ropa).
   - Animaciones: `Idle`, `Walking`, `Running`, `Jump`.
2. Importar en `Assets/Models/Player/`:
   - Seleccionar el `.fbx` → `Inspector → Rig`:
     - `Animation Type` = **Humanoid**
     - `Avatar Definition` = **Create From This Model**
     - `Optimize Game Objects` = ✅ (optimiza, y da puntos en "optimización de assets")
   - `Model → Scale` = **1.0** (que Mixamo viene en escala correcta).
   - Verificar que el pivote está **en los pies**: si no, en la pestaña `Animation` del modelo, `Configure → Pivot → Feet`.
3. Verificar que las animaciones se reproducen en el Preview del Inspector.
4. **Verificar la altura real del modelo** (de pie, de punta a pies) y comunicarla a Sebastian: define el `height` y el `center.y` del `CharacterController` (T-S3). El estándar de Mixamo es 1.8 m.
5. Entregar a Sebastian: el `.fbx` importado con las opciones ya seteadas (así él no toca el `Rig`).

> **Andres NO toca el prefab del jugador.** Entrega el `.fbx` en `Assets/Models/Player/` y Sebastian arma el prefab `Player` con la jerarquía de §1.3. Si Andres mete el humanoid directo en el prefab, se desincroniza con el `CharacterController`.

**Entregable:** humanoide importado, rig humanoid, avatar listo, altura reportada.

---

### T-A4 — Clips de animación 🟡 Medio

**Objetivo:** los clips que Sebastian mete en el Animator.

1. Descargar de Mixamo: `Idle`, `Walking`, `Running`, `Jump` (+ opcional `Death`, `Interact`).
2. Importar cada uno en `Assets/Animations/`:
   - `Rig` = **Humanoid**, `Avatar Definition` = **Create From This Model** (o el mismo avatar del modelo).
   - En la pestaña `Animation`: `Loop Time` ✅ en `Idle`/`Walk`/`Run`, ❌ en `Jump`.
3. **Renombrar los clips exactamente** como en §3.3: `Idle`, `Walk`, `Run`, `Jump`, `Interact`.
4. Opcional: un `Death`/`Caida` para cuando el personaje sale del nivel.
5. Entregar a Sebastian la lista de clips con sus nombres exactos.

**Entregable:** clips listos, con los nombres del contrato.

---

### T-A5 — Coleccionables y portal 🔴 Crítico

**Objetivo:** los objetos que dan sentido a la mecánica D1-A.

1. **Cristales** (`Cristal_01` … `Cristal_05`):
   - Modelo: un mesh simple (icosaedro / octaedro / un cristal low-poly) — puede ser geometría primitiva con material emisivo.
   - Material: transparente o emisivo (`URP/Lit` con `Emission` activado, o `Universal Render Pipeline/Unlit` con color).
   - Collider: **Trigger** (`Is Trigger` ✅).
   - Tag: **`Cristal`** (Sebastian lo declara en el `TagManager`, §3.3).
   - Nombres EXACTOS: `Cristal_01` … `Cristal_05`.
   - **Visibilidad**: un material emisivo puro es invisible en la niebla/lejos. Usar `emission` + un billboard o un halo. Un punto de luz con baja intensidad ayuda a que se vean desde lejos.
2. **Portal** (`Portal`) — ⚠️ **necesita DOS estados visuales** (decisión D1b: el portal está bloqueado hasta juntar los 5 cristales):
   - Modelo: un arco (puede reutilizarse el **arco romano** que ya está en el repo, escalado) o un mesh simple con material emisivo.
   - Collider: **Trigger** ✅. Tag: **`Meta`**. Nombre EXACTO: `Portal`.
   - **Estado "sellado"** (por defecto): material apagado / gris / sin emisión, y **sin luz** child. Se lee como "cerrado".
   - **Estado "activo"**: material emisivo (verde o del color que combine con los cristales) + una luz child (`Point Light`, `Intensity` ~2–3, alcance corto) que lo hace **visible desde lejos** en la niebla.
   - **Cómo se cambia entre estados:** dos materiales en el prefab y Sebastian los intercambia desde código (`GoalZone.Activar()` cambia el material del renderer y enciende la luz). **Andres deja los dos materiales en `Assets/Materials/` con nombres claros** (`Portal_Sellado.mat` y `Portal_Activo.mat`) y el slot del renderer listo para cambiar.
   - **No hace falta que Andres anime nada**: el cambio de estado es un swap de material + `light.enabled`. Lo hace Sebastian en una línea.
3. **Ubicación:** distribuirlos **dentro del laberinto**, en ramas distintas, para que el jugador tenga que explorar (el rubric: *"obligar al usuario a explorar"*). No todos a la vista.
4. Entregar a Sebastian: los prefabs `Assets/Prefabs/Collectibles/Collectible.prefab` y `Assets/Prefabs/Environment/Portal.prefab` ya con collider y tag.

**Entregable:** 5 cristales + 1 portal con los tags y colliders correctos, y el portal con sus dos estados visuales listos para el swap.

---

### T-A6 — Niebla, iluminación y ambiente 🟡 Medio

**Objetivo:** encender y afinar lo que el avance 1 dejó a medias.

1. **Niebla** — el avance 1 la dejó **configurada pero apagada** (`m_Fog: 0`):
   - `Window → Rendering → Lighting → Environment` → `Other Settings` → `Fog` = **Linear** o **Exponential Squared**.
   - `Fog Color` coherente con el tono del skybox (no gris por default: 0.5/0.5/0.5 desentona).
   - `Fog Density` / `Start` / `End` según el tamaño de la escena (con el terreno de 200 m: Linear de ~60 a ~180).
   - **Verificar que los cristales se sigan viendo con la niebla** (son el objetivo → no pueden desaparecer en la niebla).
2. **Iluminación**:
   - Ajustar la `Directional Light`: `Rotation` coherente con la dirección del sol del skybox, `Intensity` ~1.2–1.5, `Shadows = Soft`.
   - `Environment Lighting` → `Source` = **Skybox** (ya está: `m_AmbientMode: 0`) → el ambiente ya sale del cielo, lo cual da coherencia automática. Verificar que se vea bien.
   - Opcional: 1–2 luces puntuales cálidas en el portal y cerca de la salida, para dar profundidad.
3. **Limpieza de `Assets/Materials/`**: hay 7 materiales. Verificar que **todos** los objetos de la escena usan uno de ellos y que no quedaron materiales default grises. `Assets/Editor` → script de auditoría de materiales huérfanos (o revisión visual).
4. **Coherencia de escala de los materiales**: un `tiling` de textura que en un cubo se ve bien y en otro no → ajustar `tiling` por material según el tamaño real del objeto.

**Entregable:** niebla activa y con color, iluminación afinada, materiales coherentes.

---

### T-A7 — Props y pulido de composición 🟢 Bajo

**Objetivo:** que la escena se vea "armada" y no "suelta".

1. Distribuir props temáticos: vegetación, SETas, escombros, faroles/antorchas,estones, raíces.
   - Reutilizar los modelos ya descargados (rocas de Poliigon, musgo) con **variación de escala y rotación** — no repetir el mismo asset en el mismo ángulo.
2. Poblar el exterior del laberinto: el terreno de 200×200 está vacío alrededor del laberinto y se va a ver.
3. Distribuir los 5 cristales en **puntos que obliguen a explorar** (caminos sin salida, esquinas ocultas, detrás de la gárgola).
4. **Verificar la escala de las texturas** en el terreno: un `tiling` de `t140` que se vea bien en el terrain y mal en los muros es un problema de coherencia (10 % de estilo artístico).

**Entregable:** escena poblada y compuesta, no un laberinto flotando en el vacío.

---

## 6. Secuencia y dependencias

```
        ┌─────────────────────────┐
        │  D1 / D2 / D3 decisiones│  ← 30 min, juntos
        └────────────┬────────────┘
                     │
        ┌────────────▼────────────┐
        │  T-S1 contrato + rama   │
        └────────────┬────────────┘
                     │
        ┌────────────▼────────────┐
        │  T-S2 saneamiento       │  ← tags, layers, carpetas, TMP
        └────────────┬────────────┘
                     │
     ┌───────────────┼───────────────────┐
     │               │                   │
┌────▼─────┐  ┌──────▼───────┐  ┌────────▼───────┐
│ T-A1     │  │ T-A3         │  │ T-A5           │
│ Terrain  │  │ Humanoide    │  │ Cristales/Portal│
└────┬─────┘  └──────┬───────┘  └────────────────┘
     │               │
┌────▼─────┐        │
│ T-A2     │        │
│ Reubicar │        │
│ laberinto│        │
└────┬─────┘        │
     │               │
     └───────┬───────┘
             │  ← recién acá Sebastian puede cerrar el tuning
      ┌──────▼──────────────────────┐
      │  T-S3 Player v2  (Fase 2)   │
      │  T-S4 Ground + CameraOrbit  │
      └──────┬──────────────────────┘
             │
      ┌──────▼───────┐   ┌────────────────┐
      │ T-S5 Animator│   │ T-A4 clips ────┘
      │  + Diagrama  │
      └──────┬───────┘
             │
      ┌──────▼──────────────────────────────────┐
      │ T-S6 GameManager  │ T-S7 UI  │ T-S8 Sonido│
      └──────┬──────────────────────────────────┘
             │
      ┌──────▼───────┐        ┌─────────────────┐
      │ T-A6 niebla  │        │ T-A7 props      │
      │ e iluminación│        └─────────────────┘
      └──────┬───────┘
             │
      ┌──────▼──────────────────────────────────┐
      │  INTEGRACIÓN + PRUEBAS (§7)            │
      └──────┬──────────────────────────────────┘
             │
      ┌──────▼──────────────────────────────────┐
      │  T-S10 bugs + checklist de entrega (§8) │
      └─────────────────────────────────────────┘
```

**Tablero deKanban sugerido** (una columna por estado, un post-it por tarea):

| Para hacer | Bloqueado por | Responsable |
|---|---|---|
| T-S1, T-S2 | — | Sebastian |
| T-A1, T-A2 | D2 cerrada | Andres |
| T-A3, T-A4 | D3 cerrada | Andres |
| T-A5 | D1 cerrada + T-A2 (para ubicar en el laberinto) | Andres |
| T-S3, T-S4 | T-A2 (escala) + T-A3 (modelo) | Sebastian |
| T-S5 | T-A4 (clips) | Sebastian |
| T-S6, T-S7, T-S8 | T-S2 (tags/layers) | Sebastian |
| T-A6, T-A7 | T-A1, T-A2 | Andres |
| Integración | todo | ambos |

> **Regla de trabajo en pareja:** cada **martes y jueves** una sesión de 45 min de integración (juntos, con el Editor abierto). Es más barato que integrar al final. Durante esas sesiones se revisan los posts de "hecho" y se decide si algo se puede destrabar.

---

## 7. Checklist de integración y pruebas

> No hay infraestructura de tests automatizados en el proyecto (`com.unity.test-framework` está instalado pero vacío). Para un taller, **una checklist manual bien hecha vale más que un test mal hecho**. Si sobra tiempo al final, agregar un `PlayMode` test para el `GameManager` (ver T-S6).

### 7.1 Pruebas funcionales (Sebastián)

| # | Prueba | Resultado esperado |
|---|---|---|
| F1 | Abrir la escena y darle Play | Sin errores en la consola |
| F2 | Pantalla inicial | Aparece **"EL LABERINTO DE LA GÁRGOLA"**, objetivo y controles |
| F3 | Pulsar una tecla | Arranca el juego, el mouse queda capturado |
| F4 | WASD | El humanoide se mueve en las 4 direcciones |
| F5 | Shift | Corre más rápido que al caminar, y la animación cambia a Run |
| F6 | Espacio en el suelo | Salta, la animación cambia a Jump |
| F7 | Espacio en el aire | **No** salta (ground-check funcionando) |
| F8 | Soltar las teclas | El personaje **se detiene en seco** (sin derrapar) |
| F9 | Mover el mouse | La cámara orbita, sin saltos ni clipping en muros |
| F10 | Correr contra un muro | No lo atraviesa; suelta el `SFX_Impacto` y el puntaje baja 10 |
| F11 | Tocar un cristal | **+100**, `Cristales: X / 5` sube, suena `SFX_Recoger`, el cristal desaparece |
| F12 | **Recoger el 5º cristal** | El portal **se enciende** (material + luz) y se ve desde lejos |
| F13 | Entrar al portal **sin** los 5 cristales | **No** finaliza; avisa cuántos faltan (el trigger está activo pero el portal está sellado) |
| F14 | Entrar al portal **con** los 5 | Pantalla final con puntaje, cristales y tiempo; el juego se congela |
| F15 | Tras finalizar | **No** se puede seguir puntuando ni volver a recoger |
| F16 | Chocar muchas veces | El puntaje **nunca baja de 0** (clamp) |
| F17 | Mirar el HUD | El puntaje sube 1 por segundo mientras se juega; la suma coincide con `+100`/cristal y `−10`/impacto |
| F18 | Recorrer el laberinto | Hay un camino válido de `PuntoInicio` al `Portal` |
| F19 | Mirar los 5 cristales | Se ven desde la distancia (emisión + niebla no los borran) |

### 7.2 Pruebas visuales (Andrés)

| # | Prueba | Resultado esperado |
|---|---|---|
| V1 | Vista general | Se ve terreno con relieve, no una placa plana |
| V2 | Niebla | Activa, con color coherente con el cielo, no gris por default |
| V3 | Skybox | 6 caras, sin costuras ni rompimientos al girar la cámara |
| V4 | Iluminación | Sombras coherentes con la dirección del sol; ningún objeto en negro plano |
| V5 | Materiales | Ningún objeto con material gris por default |
| V6 | Escala | El humanoide, los muros y los props se ven del mismo "mundo" |
| V7 | UI | Los colores de la interfaz combinan con la paleta de la escena |
| V8 | Audio | La música no tapa los efectos; nada satura |
| V9 | **Portal sellado vs. activo** | Sellado: se lee como cerrado. Activo: se ve desde el otro extremo del laberinto, incluso con niebla |
| V10 | **Gárgola** | El asset que da nombre al juego se ve bien y se reconoce desde la entrada del laberinto |

### 7.3 Pruebas de entrega

| # | Prueba | Resultado esperado |
|---|---|---|
| E1 | Build de la escena | Sin errores ni warnings |
| E2 | Build ejecutable (.exe) | Arranca, se juega, llega a la meta |
| E3 | `git status` | Limpio, sin archivos sueltos |
| E4 | Rama | Todo el trabajo en `feature/taller-final`, `main` intacto |
| E5 | Historial | Commits chicos y legibles, no un commit gigante |

---

## 8. Checklist de entrega

### Contenido obligatorio según `Taller_Final.md`

| Requisito | Dónde se cumple | Estado |
|---|---|---|
| Escenario con terrain, relieve, materiales | T-A1, T-A2, T-A6 | ☐ |
| Skybox propio + fog + iluminación | T-A6 (skybox ya está) | ☐ |
| Personaje humanoide con Idle/Walk/Run/Jump | T-A3, T-A4, T-S3, T-S5 | ☐ |
| **Diagrama de animaciones** que coincide con el Animator | T-S5 (`Docs/Diagrama_Animaciones.md`) | ☐ |
| Movimiento + cámara + acción adicional (saltar/correr) | T-S3, T-S4 | ☐ |
| Colliders, triggers, CharacterController, detección de suelo | T-S3, T-S4, T-A5 | ☐ |
| Objetivo verificable | T-S6, T-A5 | ☐ |
| Puntaje real visible | T-S6, T-S7 | ☐ |
| Pantalla inicial + HUD + pantalla final | T-S7 | ☐ |
| Sonido ambiente + 2 SFX de evento | T-S8 | ☐ |
| PDF de entrega con créditos de assets | actual + actualizar | ☐ |

### Organización del proyecto (criterio "Organización y calidad", 7 %)

- ☐ `Assets/` con la estructura de §3.2, sin carpetas vacías ni sueltas.
- ☐ `Assets/Scripts/` con subcarpetas por responsabilidad (§3.2).
- ☐ Cada script tiene **una** responsabilidad y nombre de archivo = nombre de clase.
- ☐ Prefabs en `Assets/Prefabs/` con subcarpetas.
- ☐ `Terrain/`, `Animations/`, `Audio/` con los assets nuevos.
- ☐ Materiales usados por todos los objetos (nada de default gris).
- ☐ Sin objetos "sueltos" en la raíz de la escena: todo colgado de un padre con nombre (`Laberinto`, `Entorno`, `Collectibles`, `UI`).
- ☐ Nombres de objeto descriptivos y consistentes (mismo criterio en todo el proyecto).

### Git

- ☐ Rama `feature/taller-final` creada (T-S1).
- ☐ `Taller_Final.md` y `Documento_Entrega.pdf` commiteados.
- ☐ Commits por unidad de trabajo, no un commit gigante. Mensajes en Conventional Commits:
  `feat(player): switch to CharacterController with jump and run`
  `feat(ui): add start screen, HUD and end screen`
  `feat(terrain): add sculpted terrain with rocky ground material`
- ☐ `.gitignore` cubre `Assembly-CSharp*.csproj`, `*.slnx`, `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `.atl/`.
- ☐ Push a `origin` **sólo cuando los dos revisen el trabajo del otro** (la push y el PR son decisiones del equipo, no automáticas).

---

## 9. Riesgos y mitigaciones

| Riesgo | Impacto | Mitigación |
|---|---|---|
| **El terreno cambia la escala** y rompe el tuning de movimiento | Sebastian tiene que retocar todo | T-A2 se cierra **antes** de T-S3 Fase 2. T-S3 Fase 1 con assets provisorios para no perder tiempo |
| **Andres tarda en entregar el humanoide** | Sebastian queda bloqueado en el Animator | T-S3/T-S4 se pueden hacer con un cápsula como placeholder. El Animator se arma al final |
| **El Animator y el diagrama no coinciden** | Se pierden 10 % + 10 % | El diagrama se dibuja **después** del Controller, no antes. Y se revisan juntos |
| **El avance 1 tiene bugs heredados** | Se arrastran al juego final | T-S10 es explícito. El tag `Meta` es el más urgente |
| **La UI desentona con la escena** | Pierde puntos en "estilo artístico" | §3.3: Andres pasa la paleta antes de que Sebastian maquetear |
| **Se traba la entrega** por exceso de scope | No llega a tiempo | §1.3 ordena por peso en la rúbrica. Si hay que recortar, se recorta de §9 (sonido) antes que de §2/§3 |
| **Falta tiempo para el PDF de entrega** | Entrega incompleta | Actualizar el PDF **en paralelo**, no al final. Que lo escriba quien tenga los créditos de assets (Andres) |

---

## 10. Registro de decisiones

### Cerradas ✅

| # | Decisión | Alternativas | **Elegido** | Quién decide | Fecha |
|---|---|---|---|---|---|
| D1 | Mecánica de objetivo | A) cristales+portal ⭐ B) sólo portal C) puntos de interés | **A — 5 cristales + portal** | ambos | 2026-11-24 |
| D2 | Terreno | A) Unity Terrain ⭐ B) geometría | **A — Unity Terrain** | ambos | 2026-11-24 |
| D3 | Origen del humanoide | A) Mixamo ⭐ B) Mixamo+rig propio C) Starter Assets | **A — Mixamo** | ambos | 2026-11-24 |
| D4 | ¿Se conserva el laberinto de cubos? | A) sí, sobre el terrain ⭐ B) se reemplaza | **A — se conserva** (derivado de D2) | ambos | 2026-11-24 |
| D5 | Regla de puntaje | A) con penalización ⭐ B) sin penalización | **A — `+100`/cristal, `+1`/seg, `−10`/impacto, clamp a 0** | ambos | 2026-11-24 |
| D6 | ¿Enemigo o hazard? | A) no ⭐ B) enemigo con IA C) hazard sin IA | **A — ninguno** | ambos | 2026-11-24 |
| D7 | Nombre de la experiencia | (elegir) | **"El Laberinto de la Gárgola"** | ambos | 2026-11-24 |
| D8 | ¿Se usa el modelo de esfera? | A) no ⭐ B) sí, como bonus | **A — no, se descarta** (derivado de D3) | ambos | 2026-11-24 |
| **D1b** | ¿El portal exige los 5 cristales? | A) bloqueado ⭐ B) siempre abierto | **A — bloqueado hasta juntar los 5**, se enciende al recoger el último | ambos | 2026-11-24 |

### Reglas cerradas en detalle

**D5 — Puntaje**

| Regla | Valor |
|---|---|
| Cristal recogido | `+100` (5 cristales = 500 base) |
| Segundo sin impactos | `+1` |
| Impacto con muro | `−10` |
| Clamp | `Puntaje = Mathf.Max(0, Puntaje)` |

Rango esperado: ~600–900. Conecta dos criterios rúbrica: la colisión (§5) queda visible en el puntaje (§7).

**D6 — Sin enemigos ni hazards.** El laberinto y sus muros ya cubren *"superar obstáculos"* del §6. Si al final sobra tiempo, se puede agregar un hazard simple (obstáculo móvil) — **fuera del alcance planificado**.

**D1b — Portal bloqueado.** Al recoger el quinto cristal, el portal se enciende visualmente (swap de material + luz) y recién ahí finalize la partida. El trigger sigue siempre activo para poder leer *"faltan N cristales"*.

**D7 — "El Laberinto de la Gárgola".** El título ata al asset más distintivo de la escena, lo que juega a favor del 10 % de coherencia artística.

### Abiertas ⬜

| # | Decisión | Alternativas | Elegido | Quién decide | Impacto |
|---|---|---|---|---|---|
| **D9** | Fecha de entrega | | | — | Define el ritmo del plan. Sin esto no se pueden dimensionar los cortes de trabajo |

> **Sólo queda D9.** No bloquea ninguna tarea técnica: es la que define el ritmo y, si se aprieta, **qué se recorta**. El orden de recorte ya está decidido: primero **sonido** (5 %), después **polish de ambiente** (props, T-A7), antes que humanoide/animaciones (20 %) o UI (8 %), que son irrenunciables.

---

*Documento generado el 2026-11-24 sobre el commit `481714e` (avance 1). Decisiones D1–D8 y D1b cerradas el 2026-11-24; sólo queda abierta D9 (fecha de entrega). Actualizar la sección 1 si el proyecto cambia.*
