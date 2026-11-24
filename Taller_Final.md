# Taller Final

## Desarrollo de una experiencia 3D interactiva en Unity

Desarrollar una experiencia 3D interactiva en Unity que integre los conceptos como: construcción y ambientación de escenarios, incorporación de modelos 3D, personajes humanoides, animaciones, interacción mediante código, captura de entradas del usuario, colisiones, *interfaces, sonido, efectos visuales* y definición de una mecánica de juego con un objetivo verificable.

El resultado debe funcionar como una experiencia completa: el usuario inicia en una escena, recibe información sobre lo que debe hacer, controla un personaje, interactúa con el entorno, obtiene un puntaje o progreso y finalmente alcanza una condición de finalización.

## Planteamiento

Diseñe y desarrolle una escena 3D con una temática libre. Puede tratarse, por ejemplo, de un entorno natural, una exploración, una misión de recolección, un recorrido, una escena de aventura, un espacio fantástico, una simulación o cualquier otra propuesta que permita integrar los requisitos del taller.

La escena debe mantener una identidad visual coherente. Los modelos, materiales, iluminación, terreno, cielo, interfaz y efectos ambientales deben responder a una misma intención artística. No se evaluará únicamente la cantidad de objetos incorporados, sino la forma en que estos construyen un entorno reconocible y organizado.

---

## 1. Escenario 3D

Construya un escenario utilizando un Terrain de Unity o una combinación justificada de terreno y geometría desarrollada mediante otras herramientas.

El escenario deberá contener como mínimo:

- Un terreno configurado y modificado.
- Cambios de altura o relieve perceptibles.
- Materiales o texturas aplicadas al terreno.
- Objetos 3D distribuidos dentro de la escena.
- Elementos que permitan reconocer una intención artística o temática.
- Un Skybox diferente al predeterminado.
- Iluminación coherente con el entorno diseñado.
- Fog o niebla ambiental configurada de acuerdo con la escena.

Los modelos pueden provenir de fuentes externas, herramientas de generación de modelos 3D, Sketchfab o modelos desarrollados o modificados por el estudiante. Cuando sea necesario, podrán realizarse conversiones de formatos como GLB a FBX antes de incorporarlos a Unity.

No se considera suficiente descargar una escena completa y utilizarla sin modificación. El estudiante debe construir, organizar y adaptar el entorno.

---

## 2. Personaje humanoide

Incorpore al escenario por lo menos un personaje humanoide controlado por el usuario.

El personaje debe:

- Desplazarse dentro del escenario.
- Responder a entradas del usuario.
- Mantener contacto adecuado con el terreno.
- Interactuar correctamente con las colisiones del entorno.
- Incorporar varias animaciones relacionadas con sus acciones.

Como mínimo deben utilizarse las siguientes animaciones o equivalentes según la propuesta:

**Idle → Walk → Run → Jump**

Puede incorporar otras animaciones como ataque, recoger objeto, daño, saludo, caída, muerte, interacción u otras relacionadas con la experiencia diseñada.

---

## 3. Diagrama de animaciones

Antes de implementar completamente el Animator Controller, diseñe un diagrama de estados de animación.

El diagrama entregado deberá coincidir con la implementación realizada en el Animator Controller. Se espera el uso de parámetros dependiendo de las animaciones utilizadas.

---

## 4. Control y entradas del usuario

El personaje o elemento controlable debe responder al teclado, mouse o dispositivo definido para la experiencia.

Como mínimo debe existir: desplazamiento adelante/atrás + desplazamiento o rotación lateral + rotación de cámara/personaje. Debe incorporarse además por lo menos una entrada asociada con una acción diferente al desplazamiento, por ejemplo:

- Espacio → saltar
- E → interactuar
- Shift → correr
- Mouse → orientación

El movimiento debe detenerse correctamente cuando el usuario deja de proporcionar la entrada correspondiente.

---

## 5. Colliders, triggers y física

La escena debe utilizar correctamente los componentes de física estudiados durante el curso. Se deberá demostrar el uso de:

| Elemento | Uso esperado |
|---|---|
| Collider | Evitar que el personaje atraviese objetos o zonas sólidas. |
| Trigger | Detectar zonas, objetivos, elementos coleccionables o eventos. |
| Rigidbody o Character Controller | Gestionar el movimiento o interacción física según la solución implementada. |
| Detección de suelo | Determinar cuándo el personaje puede saltar o ejecutar determinadas acciones. |

Los colliders deberán ajustarse razonablemente a la geometría de los objetos. No deberán existir barreras invisibles injustificadas ni objetos claramente sólidos que puedan atravesarse.

---

## 6. Objetivo de la experiencia

La aplicación debe tener un objetivo identificable y verificable. Algunos ejemplos son:

- Recoger una determinada cantidad de objetos.
- Encontrar diferentes elementos dentro del escenario.
- Llegar a una ubicación.
- Completar un recorrido.
- Activar varios puntos.
- Superar obstáculos.
- Rescatar o localizar un objeto.
- Alcanzar una puntuación determinada.

La experiencia no debe limitarse a caminar libremente por el escenario. El objetivo seleccionado deberá obligar al usuario a explorar o interactuar con el entorno.

---

## 7. Sistema de puntaje o progreso

La interfaz deberá mostrar permanentemente algún indicador relacionado con el progreso del usuario. Por ejemplo:

```
Puntaje: 350
Objetos: 4 / 7
Objetivos encontrados: 3 / 5
```

El valor debe actualizarse durante la ejecución de acuerdo con las acciones realizadas. El puntaje no puede ser únicamente decorativo; debe cambiar como consecuencia de la lógica implementada.

---

## 8. Interfaz e información al usuario

La aplicación debe utilizar elementos de interfaz de Unity para comunicar información.

### Al iniciar

Debe aparecer un mensaje que indique al menos:

- Nombre de la experiencia
- Objetivo
- Controles principales

Ejemplo:

```
EXPLORACIÓN DEL BOSQUE
Encuentra los cinco cristales ocultos
y llega al portal de salida.

WASD: movimiento
Mouse: cámara
Shift: correr
Espacio: saltar

Presione una tecla para comenzar
```

### Durante la ejecución

Debe mostrarse el puntaje, cantidad de objetos encontrados, progreso o indicador equivalente.

### Al finalizar

Cuando se cumpla el objetivo debe aparecer un mensaje final. Ejemplo:

```
MISIÓN COMPLETADA

Cristales encontrados: 5 / 5
Puntaje final: 850
Tiempo: 02:43
```

Después de finalizar, el usuario no debería continuar acumulando puntos o activando nuevamente los mismos objetivos.

---

## 9. Sonido

La experiencia deberá incorporar sonido. Como mínimo:

- Un sonido ambiental o música de fondo.
- Dos sonidos asociados con eventos diferentes.

Por ejemplo:

- Ambiente del bosque
- Sonido al recoger objeto
- Sonido al completar la misión

El volumen debe permitir distinguir los sonidos sin generar saturación.

Los sonidos deben responder a los acontecimientos de la aplicación y no reproducirse continuamente cuando corresponden a eventos puntuales.

---

## Rúbrica de evaluación

| Criterio | Desempeño esperado | Valor |
|---|---|---|
| Escenario, Terrain y ambientación | El terreno está trabajado, existe composición espacial, se utiliza Skybox, Fog e iluminación de manera coherente con la propuesta. | 10 % |
| Estilo artístico y composición | La escena mantiene coherencia entre temática, modelos, materiales, iluminación, ambiente, interfaz y escala de los objetos. | 10 % |
| Modelos 3D y construcción de la escena | Integra y organiza adecuadamente los modelos; realiza ajustes de escala, orientación, materiales y ubicación. El escenario contiene suficiente variedad sin limitarse a una escena descargada. | 10 % |
| Humanoide y sistema de animaciones | El personaje está correctamente configurado. Integra varias animaciones y las transiciones responden a las acciones del usuario. | 10 % |
| Diagrama y Animator Controller | Existe correspondencia entre el diagrama planteado, parámetros, estados, condiciones y transiciones implementadas en Unity. | 10 % |
| Movimiento y entradas del usuario | El desplazamiento es controlable, responde correctamente al teclado/mouse y se incluye al menos una acción adicional. | 10 % |
| Colliders, triggers y física | Los objetos interactúan de forma coherente. Se utilizan colliders, triggers y el sistema de movimiento/física apropiado sin errores notorios de colisión. | 10 % |
| Objetivo, lógica y funcionalidad | Existe una mecánica clara, condición de avance y condición de finalización. Los eventos se controlan mediante código y funcionan de manera consistente. | 10 % |
| UI, puntaje e información al usuario | Existe pantalla o mensaje inicial, información de progreso o puntaje durante la ejecución y mensaje final coherente con el resultado obtenido. | 8 % |
| Sonido | Integra ambiente sonoro y efectos asociados con eventos de la experiencia, con reproducción y volumen adecuados. | 5 % |
| Organización y calidad del proyecto | Carpetas, jerarquía, nombres, scripts, prefabs y recursos están organizados. La aplicación puede ejecutarse y no presenta errores que impidan completar la experiencia. | 7 % |
| **Total** | | **100 %** |

---

