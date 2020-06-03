import json
import requests
import argparse


# You can add multiple endpoints in one API call. The payload would look like:
example_multiple_endpoints = """
{
  "endpoints": [
    "ssh",
    "ipython",
    "tensorboard"
    {
      "name": "port-40000",
      "podPort": 40000
    }
  ]
}
"""
# This script only shows adding one endpoint in one API call.
# If the endpoint exists, subsequent adding endpoint call will be ignored.
# Added endpoints can be retrieved in JobDetail


def get_parser():

    """

    Generate a parameters parser.

    """

    # parse parameters
    parser = argparse.ArgumentParser(description="Add job endpoint V2")

    # job cluster and authorization
    parser.add_argument("--cluster", type=str, default="", help="DLTS cluster")
    parser.add_argument("--team", type=str, default="", help="DLTS team")
    parser.add_argument("--userName", type=str, default="", help="Username in DLTS personal page. Your own alias")

    # job info
    parser.add_argument("--jobId", type=str, default="", help="Id of the job")
    parser.add_argument("--endpoint", type=str, default="ssh", help="\"ssh\", \"ipython\", \"tensorboard\" or port in range 40000 - 49999")

    return parser


def parametersValidation(params):

    # please add more if like
    params_not_None = [params.cluster, params.team, params.userName, params.jobId, params.endpoint]
    for param in params_not_None:
        assert param is not None and len(str(param)) != 0


def get_endpoint_payload(params):
    if params.endpoint in ["ssh", "ipython", "tensorboard"]:
        return {"endpoints": [params.endpoint]}

    port = int(params.endpoint)
    payload = {
        "endpoints": [
            {
                "name": "port-%s" % port,
                "podPort": port
            }
        ]
    }
    return payload


def add_job_endpoint(params):
    post_url = 'http://itpaether.azurewebsites.net/{0}/endpoints?userName={1}@microsoft.com'.format(params.cluster, params.userName)

    jobparams = get_endpoint_payload(params)
    jobparams['jobId'] = params.jobId
    
    print('url = {}'.format(post_url))
    print('jobparams: type={} value = {}'.format(type(jobparams), jobparams))
    
    r = requests.post(
        url=post_url,
        json=jobparams)

    print('response={}'.format(r))
    print('response_text={}'.format(r.text))


if __name__ == '__main__':
    # generate parser / parse parameters
    parser = get_parser()
    params = parser.parse_args()

    # validate params
    parametersValidation(params)

    # query job
    add_job_endpoint(params)