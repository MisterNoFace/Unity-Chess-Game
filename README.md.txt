# Progetto Unity - Gioco degli Scacchi 3D

## Descrizione del Progetto

Questo progetto consiste in un gioco degli scacchi sviluppato con **Unity**, attualmente in una fase avanzata di sviluppo. La maggior parte delle funzionalità principali è stata implementata e il gioco è pienamente giocabile per quanto riguarda il movimento dei pezzi e la gestione del turno.

### Stato Attuale

- Scacchiera 3D completa
- Modelli 3D dei pezzi integrati
- Movimento valido per tutti i pezzi
- Gestione dei turni (Bianco/Nero)
- Cattura dei pezzi
- Evidenziazione delle mosse consentite
- Interfaccia utente di base
- Sistema di rilevamento dello scacco

Funzionalità ancora da completare:

- Implementazione dello scacco matto
- Promozione del pedone
- Correzione di alcuni bug minori e ottimizzazioni

---

## Funzionalità Principali

### Sistema di Movimento

Ogni pezzo segue le regole standard degli scacchi:

- Pedone
- Torre
- Cavallo
- Alfiere
- Regina
- Re

Il sistema verifica automaticamente la validità delle mosse prima dell'esecuzione.

### Gestione della Partita

Il gioco tiene traccia di:

- Turno corrente
- Posizione dei pezzi
- Pezzi catturati
- Situazioni di scacco

### Interfaccia Utente

L'interfaccia fornisce informazioni essenziali durante la partita:

- Turno attuale
- Stato della partita
- Mosse disponibili

---

## Modelli 3D dei Pezzi

*Figura 1 - Modelli 3D dei pezzi degli scacchi.*

![Modelli 3D dei pezzi](chess-pieces.png)

---

## Gameplay

Di seguito è possibile inserire un video dimostrativo del funzionamento del gioco.

[Video Gameplay](chess-gameplay.mp4)

In alternativa:

```html
<video width="800" controls>
    <source src="chess-gameplay.mp4" type="video/mp4">
    Il tuo browser non supporta il tag video.
</video>
```

---

## Sviluppi Futuri

### Implementazione dello Scacco Matto

Aggiunta del controllo automatico delle condizioni di vittoria quando il re non dispone di mosse valide per uscire dallo scacco.

### Promozione del Pedone

Possibilità di scegliere il pezzo desiderato quando un pedone raggiunge l'ultima traversa:

- Regina
- Torre
- Alfiere
- Cavallo

### Correzioni e Miglioramenti

- Risoluzione di bug relativi a casi particolari di movimento.
- Ottimizzazione del codice.
- Miglioramento dell'interfaccia utente.
- Rifinitura delle animazioni e della presentazione generale.

---

## Tecnologie Utilizzate

- Unity
- C#
- Sistema di Input di Unity
- Modelli 3D ottimizzati per il rendering in tempo reale