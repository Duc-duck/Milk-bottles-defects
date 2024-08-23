#include "pch.h"
#include "Source_Numpy.h"

float get_median(cv::Mat input)
{
    if (input.channels() > 1) {
        return float();
    }
    std::vector<float> output;
    for (int y = 0; y < input.rows; ++y) {
        for (int x = 0; x < input.cols; ++x) {
            float value = (int)input.at<uchar>(y, x);
            if (std::find(output.begin(), output.end(), value) == output.end())
                output.push_back(value);
        }
    }
    std::sort(output.begin(), output.end());
    size_t size = output.size();
    if (size == 0)
    {
        return 0;
    }
    else
    {
        if (size % 2 == 0)
        {
            return (output[size / 2 - 1] + output[size / 2]) / 2;
        }
        else
        {
            return output[size / 2];
        }
    }
}