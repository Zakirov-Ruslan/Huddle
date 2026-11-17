import angry from "./out/angry.svg";
import celebrating from "./out/celebrating.svg";
import confused from "./out/confused.svg";
import dancing from "./out/dancing.svg";
import head from "./out/head.svg";
import loading from "./out/loading.svg";
import peeking from "./out/peeking.svg";
import relaxed from "./out/relaxed.svg";
import running from "./out/running.svg";
import sad from "./out/sad.svg";
import sleeping from "./out/sleeping.svg";
import speaking from "./out/speaking.svg";
import thinking from "./out/thinking.svg";
import walking from "./out/walking.svg";
import waving from "./out/waving.svg";

/**
 * The mascot poses live next to the source artwork (`img/mascot/out`), so editing an
 * SVG in Inkscape is enough — no regeneration step.
 */
export const mascotPoses = {
    angry,
    celebrating,
    confused,
    dancing,
    head,
    loading,
    peeking,
    relaxed,
    running,
    sad,
    sleeping,
    speaking,
    thinking,
    walking,
    waving,
} as const;

export type MascotPose = keyof typeof mascotPoses;
