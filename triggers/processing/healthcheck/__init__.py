import json
import logging
import sklearn
import azure.functions as func


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP health-check requested.')

    try:
        sklearn_engine = sklearn.__version__
    except Exception as ex:
        sklearn_engine = "unavailable"
        logging.error(ex)

    return func.HttpResponse(
        json.dumps({
            "application": "available",
            "computational_engine_version": sklearn_engine
        }),
        status_code=200
    )
