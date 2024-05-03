# TETRIS

## GAME DESIGN DOCUMENT

Creado por: María Dolores Iglesias Suárez

Versión del documento: 1.0

## HISTORIAL DE REVISIONES

| Versión | Fecha | Comentarios |
| --- | --- | --- |
| 1.00 | 03/05/2024 | 03/05/2024 |
| < versión > | < fecha > | < comentario > |

> - *Mantén el orden de versión de manera creciente*
> - *Cada versión tendrá un comentario indicando los cambios realizados*
> - *Añade nuevas filas en caso necesario*

## RESUMEN

### Concepto

> Tetris é un videoxogo de crebacabezas orixinalmente creado e programado por Alexey Pajitnov o 6 de xuño de 1984. 

> O xogo converteuse na súa época nun éxito inmediato. A día de hoxe segue a ser popular entre moitas xeracións distintas e conta con moitas variantes e evolucións. Ao ser tan coñecido e facilmente xogable, adaptouse para moitas plataformas distintas: dende vellas computadoras, máquinas de recreativos, as primeiras consolas ou ata agora en teléfonos intelixentes ou incluso en formato físico como puzzles. É tan famoso que incluso se fan competicións entre varios xogadores. 

> Tetris é un xogo arcade clásico que non evolucionou en exceso dende 1984: incorporáronse novas mecánicas, posibilidades, novos retos, deseños de pezas variados pero segue a ser un xogo destinado a todos os públicos, tanto xoves como maiores e no que a día de hoxe conta con moitísimo seguemento gracias ao factor retro ou á melancolía de quen o xogaba na súa xuventude. Que as súas mecánicas sexan tan sinxelas facilita tamén a súa gran extensión por todo o mundo. 

> A concepción desta versión 2D é para PC aínda que podría adaptarse para tablets, teléfonos e outros aparatos facendo antes axustes de gráficos, posicionamento de textos e outros elementos dentro da pantalla. 


### Puntos Clave

> O obxectivo dos xogadores é eliminar o maior número de liñas do taboleiro.
> Outra das metas é intentar que os bloques non se acumulen preto de onde saen, xa que iso provocará un bloqueo e perderemos a partida. 
> Outro é axilizar a rapidez de pensamento sobre unhas fichas que se teñen que ordear. E exercitar a creación de puzzles cada vez máis rápido.


### Género

> Arcade, Puzzles

### Público Objetivo

> Todos os públicos, moi orientado a público dos 80 e 90 que xogaron moito a Tetris na súa xuventude

### Experiencia de Juego

> Este xogo é unha viaxe aos 80 e 90 a través dunha interface moi similar aos primeiros Tetris. As mecánicas son as mesmas que o xogo clásico pero revisitado para ter un deseño máis moderno e atractivo pero parecido aos videoxogos orixinais arcade

## DISEÑO

### Metas de Diseño

> A meta é buscar unha viaxe ao pasado cunha reinterpretación, aínda así cun look moi familiar para o xogador/a, co que xogar a un dos deseños máis exitosos da historia. 
> O xogo é unha homenaxe ao Tetris clásico pero incluindo pequenas pinceladas para darlle un toque distinto: xa sexa no deseño a través das cores empregadas ou con pequenas mecánicas distintas (como o reto do mundo ou o modo Block) para ter varios modos de xogo e ir combinándoos
> O obxectivo para o equipo é rematar o deseño cun xogo completo e funcional que funcione de principio a fin e que teña unha coherencia sonora, visual e de deseño clara para o xogador/a

## MECÁNICAS DE JUEGO

### Núcleo de Juego

> O  xogador/a debe eliminar o máximo de liñas colocando as pezas sen que obstaculicen e completen liñas horizontais
> Debe manterse xogando o máximo de tempo posible
> Intentar que as pezas non bloqueen o taboleiro completo

### Flujo de Juego

> O xogo comeza ao seleccionar unha das tres opcións que se ofrecen: 
> Tetris Classic: Escollemos o nivel nunha primeira pantalla: Doado, Medio ou Difícil. Estos tres niveis marcan a velocidade á que caen as pezas. Debemos colocar as pezas que se vaian xerando automaticamente no taboleiro de maneira que poidamos crear liñas horizontais completas que poidan eliminarse de xeito limpo. Cantas máis liñas eliminemos, maior será a puntuación. Se conseguimos 999999 puntos gañamos o xogo, se se bloquean as fichas no punto de partida dende onde saen, perdemos a partida. 
> Block Tetris: Escollemos o nivel nunha primeira pantalla: Doado, Medio ou Difícil. Estos tres niveis marcan a velocidade á que caen as pezas. Ademais, tamén marcan a cantidade de pezas fixas que se atopará o xogador cando baixe as súas fichas. No nivel doado teremos 10, no medio 20 e no difícil 30. Debemos colocar as pezas que se vaian xerando automaticamente no taboleiro de maneira que poidamos crear liñas horizontais completas que poidan eliminarse de xeito limpo. Se combinamos as nosas pezas con pezas fixas tamén poderemos eliminalas cando a liña esté completa. Cantas máis liñas eliminemos, maior será a puntuación. Se conseguimos 999999 puntos gañamos o xogo, se se bloquean as fichas no punto de partida dende onde saen, perdemos a partida. 
> Reto World plantea un mapamundi no que recorremos seis países e no que cada un ten un reto distinto. A través deles imos viaxando polo mundo. Temos que 

### Fin de Juego

> Perdemos se no Tetris Classic ou Block Tetris se bloquea o punto de partida de onde saen as fichas con outras xa colocadas
> No Reto World perdemos se non conseguimos o obxectivo dentro do tempo marcado ou bloqueamos a saída de fichas con outras xa colocadas
> Tetris Classic ou Block Tetris gañamos conseguindo 999999 puntos
> Reto World gañamos acabando todos os retos e pasando todas as fases do mundo (acaba co panel de Francia)

### Física de Juego

> Os saltos no grid do tilemap de unha celda a outra (os pasos poden ser: abaixo/esqueda/dereita) a outro é total, non hai paso intermedio entre as celdas
> Non teñen gravidade de ningún tipo as celdas ou tiles (só se creou nun momento específico para unha animación da pantalla de inicio).  Tampouco contan con collider
>Tamén se permite a rotación das pezas para situalas de mellor forma no taboleiro. A rotación é a saltos de 90º e aínda que se pode facer de un lado a outro (90ª á dereita ou -90º á esquerda) tamén se pode xogar con só un tipo de xiro (á dereita a través da frecha do teclado)
> Os límites do taboleiro non se fan con colliders tampouco se non con bloqueos de código a través de C#
> Para eliminar as liñas faise unha verificación continua de se o grid contén tiles ou baldosas ocupando o sitio. Logo de verificar que a liña está completa, elimínase
> Esta verificación serve tamén para saber se unha peza choca con outra e que non poidan solaparse entre elas
> A maiores de eliminarse a liña completa, báixanse todas as superiores para ocupar a liña ou liñas eliminadas
> No nivel de Block úsase a mesma lóxica pero cun engadido ao inicio. Aquí os pasos: 
> No mesmo tilemap no que se sitúan as pezas, hai unha zona enteira de pezas bloqueantes pintadas (faise así para evitar dende o inicio a zona dende onde poden caer as pezas)
> Ao seleccionar a dificultade polo xogador, quedan finalmente no taboleiro X pezas fixas (10 para o nivel doado, 20 para o medio e 30 para o difícil)
> A verificación continua de se hai pezas de bloques de cemento situadas no taboleiro segue a mesma mecánica que no modo clásico. E é idéntica tamén para a eliminación de filas xa que non fai distinción entre bloques de cemento e bloques de pezas. A diferencia diso é que se non eliminamos os bloques, o nivel si que se fai demasiado difícil sempre
> Hai un bloque adicional “fantasma” que funciona como pista constante para o xogador de onde pode situarse a peza no fondo do taboleiro. Esta peza verifica constantemente que na peza máis baixa que atopa non hai outra peza situada. Por exemplo no modo Block é moi útil porque vemos facilmente onde se vai a bloquear a peza ao comezo da baixada se ten un bloque de cemento no recorrido


### Controles

> A interacción básica é a do xogador/a coas pezas do taboleiro e que poden facer o seguinte: 
> Moverse aos lados
> Moverse cara abaixo
> Xirar 360º en saltos de 90º en 90º
> Soltar a peza para que baixe de repente para abaixo
> Esta interacción pode facerse desta forma co teclado da computadora: 
>Coas teclas A/S/D/Q/E/ESPACIO: 
    A: Mover á esquerda
    D: Mover á dereita
    Q: Xiramos a un lado
    E: Xiramos ao outro lado
    S: Baixamos máis rápido
    Space: Hard drop para o fondo
> Coas teclas de flechas: 
    ←: Mover á esquerda
    →: Mover á dereita
     ↑ : Xiramos a un lado
     ↓ :Baixamos máis rápido
    Space: Hard drop para o fondo


## MUNDO DEL JUEGO

### Descripción General

> A apariencia básica é moi minimalista, emulando un xogo de arcade clásico. Inclúense pequenos menús e submenús moi claros e concisos, sen demasiados elementos para que a navegación sexa o máis sinxela posible. 
> Está moi orientado a chegar moi rápido á partida que desexemos e con fondos oscuros para manter a coherencia en todos os mundos. 
> O único que cambia neste aspecto son os viaxes polo mapamundi que teñen cartelas introductorias e de final de cores (coas cores dos tetrominos), as pantallas de game over ou pausa e os de inicio de xogo de Classic, Block e Mundo con selector de niveles ou paso intermedio para arrancar o reto do mundo
> O único que cambia neste aspecto son os viaxes polo mapamundi que teñen cartelas introductorias e de final de cores (coas cores dos tetrominos), as pantallas de game over ou pausa e os de inicio de xogo de Classic, Block e Mundo con selector de niveles ou paso intermedio para arrancar o reto do mundo
> O único que cambia neste aspecto son os viaxes polo mapamundi que teñen cartelas introductorias e de final de cores (coas cores dos tetrominos), as pantallas de game over ou pausa e os de inicio de xogo de Classic, Block e Mundo con selector de niveles ou paso intermedio para arrancar o reto do mundo

### HUD

> Na pantalla do xogador sempre se mostran varias cousas ao lado do taboleiro de xogo: 
    Nivel
    Puntos
    Tempo
    Peza que virá despois
    Un botón co que activamos e desactivamos a música
> Ao final da partida Classic ou Block podemos gardar a información da nosa partida dun documento json que pode cargarse a posteriori na opción no nivel principal Scores. Aí veremos as 10 máximas puntuacións almacenadas no xogo.
Ademais, na opción de Reto World, pódese ver un par de campos a maiores para mellorar a xogabilidade entre niveis: 
    Nivel
    Puntos
    Tempo
    Peza que virá despois
    Botón para activar e desactivar música
    Vidas
    Liñas eliminadas (xa que hai retos nos que é determinante)
> Ao inicio de cada nivel deste reto, móstrase a través dun panel informativo qué obxectivo se debe cumplir para pasar o reto en cada país. 

## ARTE

### Metas de Arte

> A interface deste xogo é moi similar á clásica creada hai 40 anos. O que se buscou é crear familiaridade co xogador para que ningún elemento lle resultase estrano ou demasiado novedoso. 
> As metas de arte son simples: buscar un deseño simplificado e elegante do Tetris Clásico con cores máis modernos, liñas claras e cunha tipografía similar á do xogo de 1984 que marca o paso entre menús, botóns e información para o xogador. 
> Hai unha clara búsqueda de coherencia entre as cores: negro, gris, e as cores dos tetrominos que serven para algúns elementos: cyan, vermello, amarelo, vermello, verde, azul e laranxa

### Assets de Arte

> Creáronse todos os tiles e tetrominos de cero a través de sprites nun programa externo
> Tamén se creou o grid ou taboleiro de xogo
> E para o mapamundi fondos con imaxes 8bit de distintas cidades do país que representan
> Animación de inicio de xogo. Isto ía ser unha cortinilla entre niveis pero quedou moi longa: Animei todas as pezas con prefabs nun taboleiro base e logo cun timeline creei a animación, puxen efectos de son e activei a pista do título ao final. Como quedou longa e finaliza co taboleiro completo cheo pareceume unha boa carta de presentación para arrancar o xogo
> Animación fondo Menú principal: A través de código e os tiles, creei unha animación na que os tiles van caen pouco a pouco para reencher o fondo e que fora máis bonito. Crearonse colliders e rigidbodies individuais para que non se solaparan en exceso e seguiran as liñas do tilemap
> Animación dos bordes cambiando de cor.


## AUDIO

### Metas de Audio

> A idea xeral en canto ao son é emular a versión antiga de Tetris. Cos efectos de sonido, imitar ou homenaxear a outros xogos de arcade con sons limpos, secos e curtos. 
> En canto á música hai unha clara ambientación do Tetris clásico e con outras versións como a piano, máis orquestal... E cada un dos mundos ten unha musicalización distinta dependendo do país para que nos axuden a ambientar mellor o mapa.

### Assets de Audio

> Efectos de son: 
    Limpar liña
    Game Over
    Subida de nivel
    Peza colocada no taboleiro
    Son de victoria
> Músicas 
> Ao contar con tantísimas versións antigas de Tetris contamos cun bucle clásico de fondo para os modos Classic e Block.
>  Unha versión a piano máis suave para as puntuacións e algo máis medieval con órganos e campanas para as instruccións
> Para o mapamundi tempos unha versión de canción distinta por cada país: percusións para Exipto, guitarras acústicas e algunha percusión para México, algo máis orquestal e con órganos clásicos para Rusia, unha pequena banda de rock para EE.UU, un acordeón para Francia ou instrumentos de corda chinos para a Gran Muralla

## DETALLES TÉCNICOS

### Plataformas Objetivo

> Windows

### Herramientas de Desarrollo

> Unity
> GarageBand
> OpenIA Chat GPT
> Perplexity
> Photoshop
