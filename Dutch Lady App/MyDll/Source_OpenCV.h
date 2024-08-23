#pragma once

#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/dnn/dnn.hpp>
#include <iostream>
#include <algorithm>
#include "Source_Numpy.h"

#define EXPORTED_METHOD extern "C" __declspec(dllexport)
using namespace cv;
using namespace std;

struct ImageInfo
{
    unsigned char* data;
    int size;
    int elementSize;
};

struct Deform
{
    int wrinkle_area;
    float circularity;
};

struct ContoursInfo
{
    double Area;
    int ContourId;
};

class Custom_Threshold : public ParallelLoopBody
{
private:
    cv::Mat img;
    cv::Mat& retVal;
    double min;
    double max;
public:
    Custom_Threshold(Mat image, Mat& output, double minValue, double maxValue)
        : img(image), retVal(output), min(minValue), max(maxValue) {}

    virtual void operator()(const cv::Range& range) const;
};

int Find_ThreshValue(Mat image, int NonZeroPixels, int thresh_default, double ratio_otsu_water);

ContoursInfo GetMaxContours(std::vector<std::vector<cv::Point>> contours);

void Auto_Canny(cv::Mat Input, cv::Mat& Output, float sigma_min, float sigma_max);

cv::Point getContour_Center(std::vector<cv::Point> contour);

void moveContours(std::vector<std::vector<cv::Point>>& contours, cv::Point move);

void moveContours(std::vector<std::vector<cv::Point>> contours, std::vector<std::vector<cv::Point>>& output, cv::Point move);

float outer_radius(std::vector<cv::Point> contour);

float roundness(std::vector<cv::Point> contour);

bool IsClosedContour(std::vector<cv::Point> contour);

std::vector<cv::Point> Deform_Contour(std::vector<cv::Point> contour, float shrink_or_expand);

float getAngle(cv::Point2f* rect_points, std::vector<cv::Point2f>& points_select);

float euclideanDist(cv::Point2f& a, cv::Point2f& b);