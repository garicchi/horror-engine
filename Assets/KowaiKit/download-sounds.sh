#!/bin/bash -e

SCRIPT_PATH=$(cd $(dirname $0); pwd)

echo "効果音をダウンロードします"

mkdir -p ${SCRIPT_PATH}/Sounds

curl https://soundeffect-lab.info/sound/various/mp3/walk-leather-shoes1.mp3 -o ${SCRIPT_PATH}/Sounds/survivor-walk.mp3

curl https://soundeffect-lab.info/sound/various/mp3/walk-gymnasium1.mp3 -o ${SCRIPT_PATH}/Sounds/enemy-walk.mp3

curl https://soundeffect-lab.info/sound/anime/mp3/scream-woman1.mp3 -o ${SCRIPT_PATH}/Sounds/scream.mp3

echo "${SCRIPT_PATH}/Sounds に保存完了しました"