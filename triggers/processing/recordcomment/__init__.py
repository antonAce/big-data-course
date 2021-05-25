import uuid
import json
import nltk
import logging
import azure.functions as func

from joblib import load
from nltk.stem import WordNetLemmatizer

from ..db import *

nltk.download('wordnet')
nltk.download('punkt')

vectorizer = load('.models/comment_nlp_vectorizer.bin')
classifier = load('.models/comment_nlp_classifier.bin')
encoder = load('.models/comment_nlp_encoder.bin')


def predict_labels(features: str) -> list:
    tags = encoder.inverse_transform(
        classifier.predict(
            vectorizer.transform([features])
        )
    )

    return tags.tolist()[0]


def remove_punkt(word):
    for punkt in "?:!.,;'":
        word = word.replace(punkt, "")
    
    return word


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP comment recording requested.')

    comment_id = uuid.uuid4()

    try:
        req_body = req.get_json()
    except ValueError:
        return func.HttpResponse(
            json.dumps({
                "message": "Request body is missing."
            }),
            status_code=400
        )
    else:
        app_id = req_body.get('app_id')
        nickname = req_body.get('nickname')
        comment = req_body.get('comment')

    if comment is None or nickname is None or app_id is None:
        return func.HttpResponse(
            json.dumps({
                "message": "Required body parameters are missing."
            }),
            status_code=400
        )

    lemmatizer = WordNetLemmatizer()

    tokens = comment.split(" ")
    no_puckt = [remove_punkt(word) for word in tokens]
    words = [word.lower() for word in no_puckt]
    lemmas = [lemmatizer.lemmatize(word) for word in words]

    finalized_sentence = " ".join(lemmas)
    user_reaction = predict_labels(finalized_sentence)

    response = {
        "id": str(comment_id),
        "app_id": int(app_id),
        "nickname": nickname,
        "comment": comment,
        "user_reaction": user_reaction
    }

    insert_query = SimpleStatement("""
        INSERT INTO gamestore.comments (id, app_id, nickname, date_time, containment, reaction)
        VALUES (%s, %s, %s, toTimeStamp(now()), %s, %s)
    """, consistency_level=ConsistencyLevel.QUORUM)

    session.execute(insert_query, (response["id"], response["app_id"], response["nickname"],
                                   response["comment"], response["user_reaction"]))

    return func.HttpResponse(
        json.dumps(response),
        status_code=200
    )
