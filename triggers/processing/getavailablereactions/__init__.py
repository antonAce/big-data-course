import json
import logging
import azure.functions as func

from joblib import load


encoder = load('.models/comment_nlp_encoder.bin')


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP get available reactions requested.')

    if not encoder:
        return func.HttpResponse(
            json.dumps({
                "message": "Encoder is missing."
            }),
            status_code=400
        )

    return func.HttpResponse(
        json.dumps({
            "classes": list(encoder.classes_)
        }),
        status_code=200
    )
